"use strict";
// All image composition happens here using sharp.
// We build a background, place the screenshot, optional frame, then text.
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.renderSlide = renderSlide;
const fs_1 = __importDefault(require("fs"));
const sharp_1 = __importDefault(require("sharp"));
async function renderSlide(input) {
    const background = await createBackground(input.theme, input.layout);
    const screenshot = await resizeScreenshot(input.inputPath, input.layout.deviceArea.width, input.layout.deviceArea.height, input.cropMode);
    const composites = [];
    composites.push({
        input: screenshot,
        left: input.layout.deviceArea.x,
        top: input.layout.deviceArea.y
    });
    if (input.framePath && fs_1.default.existsSync(input.framePath)) {
        const frameBuffer = await (0, sharp_1.default)(input.framePath)
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
    const base = (0, sharp_1.default)({
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
    await (0, sharp_1.default)(input.outputPath)
        .resize(previewWidth, previewHeight, { fit: "fill" })
        .png()
        .toFile(input.previewPath);
}
async function createBackground(theme, layout) {
    const svg = theme.type === "solid"
        ? createSolidSvg(theme.color, layout.output.width, layout.output.height)
        : createLinearGradientSvg(theme.color, theme.color2 || theme.color, layout.output.width, layout.output.height);
    return (0, sharp_1.default)(Buffer.from(svg))
        .resize(layout.output.width, layout.output.height, { fit: "fill" })
        .png()
        .toBuffer();
}
function createSolidSvg(color, width, height) {
    return `<?xml version="1.0" encoding="UTF-8"?>
<svg width="${width}" height="${height}" viewBox="0 0 ${width} ${height}" xmlns="http://www.w3.org/2000/svg">
  <rect width="${width}" height="${height}" fill="${color}" />
</svg>`;
}
function createLinearGradientSvg(color1, color2, width, height) {
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
async function resizeScreenshot(inputPath, width, height, cropMode) {
    const fit = cropMode === "cover" ? "cover" : "contain";
    // Transparent background keeps letterboxing invisible on the background.
    return (0, sharp_1.default)(inputPath)
        .resize(width, height, {
        fit,
        background: { r: 0, g: 0, b: 0, alpha: 0 }
    })
        .png()
        .toBuffer();
}
async function createTextOverlay(headline, subheadline, theme, layout) {
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
    return (0, sharp_1.default)(Buffer.from(svg))
        .resize(layout.output.width, layout.output.height, { fit: "fill" })
        .png()
        .toBuffer();
}
function wrapText(text, maxWidth, fontSize) {
    // Rough estimate: average character is about 0.55 of font size.
    // This is not perfect but works well enough for short headlines.
    const maxChars = Math.max(10, Math.floor(maxWidth / (fontSize * 0.55)));
    const words = text.split(/\s+/);
    const lines = [];
    let current = "";
    for (const word of words) {
        const next = current ? `${current} ${word}` : word;
        if (next.length > maxChars && current) {
            lines.push(current);
            current = word;
        }
        else {
            current = next;
        }
    }
    if (current) {
        lines.push(current);
    }
    return lines;
}
function escapeXml(value) {
    return value
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/\"/g, "&quot;")
        .replace(/'/g, "&apos;");
}
