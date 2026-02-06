"use strict";
// Calm background themes. Keep it simple: solid or gentle gradients.
Object.defineProperty(exports, "__esModule", { value: true });
exports.Themes = void 0;
exports.getTheme = getTheme;
exports.Themes = [
    {
        name: "mist",
        type: "linear",
        color: "#E8F2F5",
        color2: "#F7FBFD",
        headlineColor: "#12323A",
        subheadlineColor: "#2C4A52"
    },
    {
        name: "breeze",
        type: "linear",
        color: "#EAF4FF",
        color2: "#F9FCFF",
        headlineColor: "#1E2F44",
        subheadlineColor: "#3A516B"
    },
    {
        name: "dawn",
        type: "linear",
        color: "#F3F0EA",
        color2: "#FFFDF8",
        headlineColor: "#3A2F25",
        subheadlineColor: "#5A4B3E"
    },
    {
        name: "seafoam",
        type: "linear",
        color: "#E7F5F1",
        color2: "#F6FFFD",
        headlineColor: "#153C35",
        subheadlineColor: "#2B5A52"
    },
    {
        name: "slate",
        type: "linear",
        color: "#E9EEF2",
        color2: "#F7FAFC",
        headlineColor: "#22303B",
        subheadlineColor: "#3B4D5C"
    }
];
function getTheme(name) {
    const found = exports.Themes.find(t => t.name === name);
    if (!found) {
        return exports.Themes[0];
    }
    return found;
}
