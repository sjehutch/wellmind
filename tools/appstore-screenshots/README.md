# App Store Screenshot Composer

This tool generates clean App Store screenshots from raw iOS simulator captures.

## Where to put inputs

Drop your raw simulator screenshots here:

- `artifacts/ios/screenshots/`

Optional frame overlay (transparent PNG):

- `artifacts/ios/frames/iphone-67.png`

If the frame is missing, the tool will render without a frame.

## Config file

Edit slides and text here:

- `tools/appstore-screenshots/appstore.config.json`

Each slide supports:
- `inputFile`
- `headline`
- `subheadline` (optional)
- `theme`
- `outputName`
- `cropMode` (`contain` or `cover`)

## How to run

From the repo root:

```
cd tools/appstore-screenshots
npm install
npm run appstore:build
```

Optional watch mode:

```
npm run appstore:watch
```

## Output

Final images:
- `artifacts/ios/appstore/6.7/`

Quick previews:
- `artifacts/ios/appstore/previews/`

## Notes

- Output size is fixed at 1284x2778 (iPhone 6.7 inch).
- The tool never edits your app code.
- Everything is deterministic and repeatable.
