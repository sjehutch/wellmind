// Loads and validates the appstore.config.json file.

import fs from "fs";
import path from "path";

export type CropMode = "contain" | "cover";

export interface SlideConfig {
  inputFile: string;
  headline: string;
  subheadline?: string;
  theme: string;
  outputName: string;
  cropMode?: CropMode;
}

export interface AppStoreConfig {
  outputSize: "iphone-67";
  slides: SlideConfig[];
}

export function loadConfig(configPath: string): AppStoreConfig {
  if (!fs.existsSync(configPath)) {
    throw new Error(`Config file not found: ${configPath}`);
  }

  const raw = fs.readFileSync(configPath, "utf8");
  const parsed = JSON.parse(raw) as AppStoreConfig;

  validateConfig(parsed);

  return parsed;
}

function validateConfig(config: AppStoreConfig): void {
  if (!config.outputSize) {
    throw new Error("config.outputSize is required");
  }

  if (!Array.isArray(config.slides) || config.slides.length === 0) {
    throw new Error("config.slides must be a non-empty array");
  }

  config.slides.forEach((slide, index) => {
    if (!slide.inputFile) {
      throw new Error(`slides[${index}].inputFile is required`);
    }
    if (!slide.headline) {
      throw new Error(`slides[${index}].headline is required`);
    }
    if (!slide.theme) {
      throw new Error(`slides[${index}].theme is required`);
    }
    if (!slide.outputName) {
      throw new Error(`slides[${index}].outputName is required`);
    }

    if (slide.cropMode && slide.cropMode !== "contain" && slide.cropMode !== "cover") {
      throw new Error(`slides[${index}].cropMode must be "contain" or "cover"`);
    }
  });
}

export function getDefaultCropMode(slide: SlideConfig): CropMode {
  return slide.cropMode ?? "contain";
}

export function resolveInputPath(projectRoot: string, inputFile: string): string {
  return path.join(projectRoot, "artifacts", "ios", "screenshots", inputFile);
}
