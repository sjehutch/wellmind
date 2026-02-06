// All image composition happens here using sharp.
// We build a background, place the screenshot, optional frame, then text.

import fs from "fs";
import path from "path";
import sharp, { Sharp } from "sharp";
import { LayoutSpec } from "./layout";
import { Theme } from "./themes";
import { CropMode } from "./config";

export interface RenderInput {
  inputPath: string;
  outputPath: string;
  previewPath: string;
  headline: string;
  subheadline?: string;
  theme: Theme;
  cropMode: CropMode;
  framePath?: string;
  layout: LayoutSpec;
}

export async function renderSlide(input: RenderInput): Promise<void> {
  const background = await createBackground(input.theme, input.layout);

  const screenshot = await resizeScreenshot(
    input.inputPath,
    input.layout.deviceArea.width,
    input.layout.deviceArea.height,
    input.cropMode
  );

  const composites: sharp.OverlayOptions[] = [];

  composites.push({
    input: screenshot,
    left: input.layout.deviceArea.x,
    top: input.layout.deviceArea.y
  });

  if (input.framePath && fs.existsSync(input.framePath)) {
    const frameBuffer = await sharp(input.framePath)
      .resize(input.layout.deviceArea.width, input.layout.deviceArea.height, {
        fit: "fill"
      })
      .toBuffer();

    composites.push({
      input: frameBuffer,
      left: input.layout.deviceArea.x,
      top: input.layout.deviceArea.y
    });
  }

  const textOverlay = await createTextOverlay(input.headline, input.subheadline, input.theme, input.layout);
  composites.push({ input: textOverlay, left: 0, top: 0 });

  const base = sharp({
    create: {
      width: input.layout.output.width,
      height: input.layout.output.height,
      channels: 4,
      background: { r: 0, g: 0, b: 0, alpha: 1 }
    }
  });

  const finalImage = base.composite([
    { input: background, left: 0, top: 0 },
    ...composites
  ]);
  await finalImage.png().toFile(input.outputPath);

  // Preview is just a smaller copy for faster iteration.
  const previewWidth = Math.round(input.layout.output.width * input.layout.output.previewScale);
  const previewHeight = Math.round(input.layout.output.height * input.layout.output.previewScale);

  await sharp(input.outputPath)
    .resize(previewWidth, previewHeight, { fit: "fill" })
    .png()
    .toFile(input.previewPath);
}

async function createBackground(theme: Theme, layout: LayoutSpec): Promise<Buffer> {
  const svg = theme.type === "solid"
    ? createSolidSvg(theme.color, layout.output.width, layout.output.height)
    : createLinearGradientSvg(theme.color, theme.color2 || theme.color, layout.output.width, layout.output.height);

  return sharp(Buffer.from(svg))
    .resize(layout.output.width, layout.output.height, { fit: "fill" })
    .png()
    .toBuffer();
}


function createSolidSvg(color: string, width: number, height: number): string {
  return `<?xml version="1.0" encoding="UTF-8"?>
<svg width="${width}" height="${height}" viewBox="0 0 ${width} ${height}" xmlns="http://www.w3.org/2000/svg">
  <rect width="${width}" height="${height}" fill="${color}" />
</svg>`;
}

function createLinearGradientSvg(color1: string, color2: string, width: number, height: number): string {
  return `<?xml version="1.0" encoding="UTF-8"?>
<svg width="${width}" height="${height}" viewBox="0 0 ${width} ${height}" xmlns="http://www.w3.org/2000/svg">
  <defs>
    <linearGradient id="bg" x1="0" y1="0" x2="0" y2="1">
      <stop offset="0%" stop-color="${color1}" />
      <stop offset="100%" stop-color="${color2}" />
    </linearGradient>
  </defs>
  <rect width="${width}" height="${height}" fill="url(#bg)" />
</svg>`;
}

async function resizeScreenshot(
  inputPath: string,
  width: number,
  height: number,
  cropMode: CropMode
): Promise<Buffer> {
  const fit: keyof sharp.FitEnum = cropMode === "cover" ? "cover" : "contain";

  // Transparent background keeps letterboxing invisible on the background.
  return sharp(inputPath)
    .resize(width, height, {
      fit,
      background: { r: 0, g: 0, b: 0, alpha: 0 }
    })
    .png()
    .toBuffer();
}

async function createTextOverlay(
  headline: string,
  subheadline: string | undefined,
  theme: Theme,
  layout: LayoutSpec
): Promise<Buffer> {
  const headlineLines = wrapText(headline, layout.textArea.width, layout.headlineFontSize);
  const subLines = subheadline
    ? wrapText(subheadline, layout.textArea.width, layout.subheadlineFontSize)
    : [];

  const headlineLineHeight = layout.headlineFontSize * layout.headlineLineHeight;
  const subLineHeight = layout.subheadlineFontSize * layout.subheadlineLineHeight;

  const headlineBlockHeight = headlineLines.length * headlineLineHeight;
  const subBlockHeight = subLines.length > 0 ? (subLines.length * subLineHeight) + 10 : 0;

  const headlineStartY = layout.textArea.y + layout.headlineFontSize;
  const subStartY = headlineStartY + headlineBlockHeight + 6;

  const svg = `<?xml version="1.0" encoding="UTF-8"?>
<svg width="${layout.output.width}" height="${layout.output.height}" viewBox="0 0 ${layout.output.width} ${layout.output.height}" xmlns="http://www.w3.org/2000/svg">
  <style>
    .headline {
      font-family: Helvetica, Arial, sans-serif;
      font-size: ${layout.headlineFontSize}px;
      font-weight: 700;
      fill: ${theme.headlineColor};
    }
    .subheadline {
      font-family: Helvetica, Arial, sans-serif;
      font-size: ${layout.subheadlineFontSize}px;
      font-weight: 400;
      fill: ${theme.subheadlineColor};
    }
  </style>
  <text class="headline" x="${layout.textArea.x}" y="${headlineStartY}">
    ${headlineLines.map((line, i) => {
      const dy = i === 0 ? 0 : headlineLineHeight;
      return `<tspan x="${layout.textArea.x}" dy="${dy}">${escapeXml(line)}</tspan>`;
    }).join("")}
  </text>
  ${subLines.length > 0 ? `
  <text class="subheadline" x="${layout.textArea.x}" y="${subStartY}">
    ${subLines.map((line, i) => {
      const dy = i === 0 ? 0 : subLineHeight;
      return `<tspan x="${layout.textArea.x}" dy="${dy}">${escapeXml(line)}</tspan>`;
    }).join("")}
  </text>
  ` : ""}
</svg>`;

  return sharp(Buffer.from(svg))
    .resize(layout.output.width, layout.output.height, { fit: "fill" })
    .png()
    .toBuffer();
}

function wrapText(text: string, maxWidth: number, fontSize: number): string[] {
  // Rough estimate: average character is about 0.55 of font size.
  // This is not perfect but works well enough for short headlines.
  const maxChars = Math.max(10, Math.floor(maxWidth / (fontSize * 0.55)));

  const words = text.split(/\s+/);
  const lines: string[] = [];
  let current = "";

  for (const word of words) {
    const next = current ? `${current} ${word}` : word;
    if (next.length > maxChars && current) {
      lines.push(current);
      current = word;
    } else {
      current = next;
    }
  }

  if (current) {
    lines.push(current);
  }

  return lines;
}

function escapeXml(value: string): string {
  return value
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/\"/g, "&quot;")
    .replace(/'/g, "&apos;");
}
