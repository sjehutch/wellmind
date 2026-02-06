"use strict";
// Layout math for App Store screenshots.
// Everything is kept in one place so it is easy to tweak later.
Object.defineProperty(exports, "__esModule", { value: true });
exports.getOutputSpec = getOutputSpec;
exports.buildLayout = buildLayout;
function getOutputSpec(name) {
    if (name === "iphone-67") {
        return {
            name,
            width: 1284,
            height: 2778,
            previewScale: 0.5
        };
    }
    throw new Error(`Unsupported output size: ${name}`);
}
function buildLayout(output) {
    // These numbers are tuned for iPhone 6.7 inch App Store screenshots.
    // Adjust them if you want more or less breathing room.
    const safeSide = 96;
    const safeTop = 120;
    const safeBottom = 120;
    const textAreaHeight = 360;
    const textGap = 40;
    const deviceAreaY = safeTop + textAreaHeight + textGap;
    const deviceAreaHeight = output.height - deviceAreaY - safeBottom;
    const deviceAreaWidth = output.width - safeSide * 2;
    return {
        output,
        safeSide,
        safeTop,
        safeBottom,
        textAreaHeight,
        textGap,
        deviceArea: {
            x: safeSide,
            y: deviceAreaY,
            width: deviceAreaWidth,
            height: deviceAreaHeight
        },
        textArea: {
            x: safeSide,
            y: safeTop,
            width: output.width - safeSide * 2
        },
        headlineFontSize: 72,
        subheadlineFontSize: 40,
        headlineLineHeight: 1.15,
        subheadlineLineHeight: 1.25
    };
}
