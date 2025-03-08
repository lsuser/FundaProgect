"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.compareBuffersOrStrings = compareBuffersOrStrings;
exports.getComparator = getComparator;
var _utilsBundle = require("../utilsBundle");
var _pixelmatch = _interopRequireDefault(require("../third_party/pixelmatch"));
var _compare = require("../image_tools/compare");
function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }
/**
 * Copyright 2017 Google Inc. All rights reserved.
 * Modifications copyright (c) Microsoft Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// @ts-ignore

function getComparator(mimeType) {
  if (mimeType === 'image/png') return compareImages.bind(null, 'image/png');
  if (mimeType === 'image/jpeg') return compareImages.bind(null, 'image/jpeg');
  if (mimeType === 'text/plain') return compareText;
  return compareBuffersOrStrings;
}
const JPEG_JS_MAX_BUFFER_SIZE_IN_MB = 5 * 1024; // ~5 GB

function compareBuffersOrStrings(actualBuffer, expectedBuffer) {
  if (typeof actualBuffer === 'string') return compareText(actualBuffer, expectedBuffer);
  if (!actualBuffer || !(actualBuffer instanceof Buffer)) return {
    errorMessage: 'Actual result should be a Buffer or a string.'
  };
  if (Buffer.compare(actualBuffer, expectedBuffer)) return {
    errorMessage: 'Buffers differ'
  };
  return null;
}
function compareImages(mimeType, actualBuffer, expectedBuffer, options = {}) {
  var _options$comparator, _ref;
  if (!actualBuffer || !(actualBuffer instanceof Buffer)) return {
    errorMessage: 'Actual result should be a Buffer.'
  };
  validateBuffer(expectedBuffer, mimeType);
  let actual = mimeType === 'image/png' ? _utilsBundle.PNG.sync.read(actualBuffer) : _utilsBundle.jpegjs.decode(actualBuffer, {
    maxMemoryUsageInMB: JPEG_JS_MAX_BUFFER_SIZE_IN_MB
  });
  let expected = mimeType === 'image/png' ? _utilsBundle.PNG.sync.read(expectedBuffer) : _utilsBundle.jpegjs.decode(expectedBuffer, {
    maxMemoryUsageInMB: JPEG_JS_MAX_BUFFER_SIZE_IN_MB
  });
  const size = {
    width: Math.max(expected.width, actual.width),
    height: Math.max(expected.height, actual.height)
  };
  let sizesMismatchError = '';
  if (expected.width !== actual.width || expected.height !== actual.height) {
    sizesMismatchError = `Expected an image ${expected.width}px by ${expected.height}px, received ${actual.width}px by ${actual.height}px. `;
    actual = resizeImage(actual, size);
    expected = resizeImage(expected, size);
  }
  const diff = new _utilsBundle.PNG({
    width: size.width,
    height: size.height
  });
  let count;
  if (options.comparator === 'ssim-cie94') {
    count = (0, _compare.compare)(expected.data, actual.data, diff.data, size.width, size.height, {
      // All ΔE* formulae are originally designed to have the difference of 1.0 stand for a "just noticeable difference" (JND).
      // See https://en.wikipedia.org/wiki/Color_difference#CIELAB_%CE%94E*
      maxColorDeltaE94: 1.0
    });
  } else if (((_options$comparator = options.comparator) !== null && _options$comparator !== void 0 ? _options$comparator : 'pixelmatch') === 'pixelmatch') {
    var _options$threshold;
    count = (0, _pixelmatch.default)(expected.data, actual.data, diff.data, size.width, size.height, {
      threshold: (_options$threshold = options.threshold) !== null && _options$threshold !== void 0 ? _options$threshold : 0.2
    });
  } else {
    throw new Error(`Configuration specifies unknown comparator "${options.comparator}"`);
  }
  const maxDiffPixels1 = options.maxDiffPixels;
  const maxDiffPixels2 = options.maxDiffPixelRatio !== undefined ? expected.width * expected.height * options.maxDiffPixelRatio : undefined;
  let maxDiffPixels;
  if (maxDiffPixels1 !== undefined && maxDiffPixels2 !== undefined) maxDiffPixels = Math.min(maxDiffPixels1, maxDiffPixels2);else maxDiffPixels = (_ref = maxDiffPixels1 !== null && maxDiffPixels1 !== void 0 ? maxDiffPixels1 : maxDiffPixels2) !== null && _ref !== void 0 ? _ref : 0;
  const ratio = Math.ceil(count / (expected.width * expected.height) * 100) / 100;
  const pixelsMismatchError = count > maxDiffPixels ? `${count} pixels (ratio ${ratio.toFixed(2)} of all image pixels) are different.` : '';
  if (pixelsMismatchError || sizesMismatchError) return {
    errorMessage: sizesMismatchError + pixelsMismatchError,
    diff: _utilsBundle.PNG.sync.write(diff)
  };
  return null;
}
function validateBuffer(buffer, mimeType) {
  if (mimeType === 'image/png') {
    const pngMagicNumber = [137, 80, 78, 71, 13, 10, 26, 10];
    if (buffer.length < pngMagicNumber.length || !pngMagicNumber.every((byte, index) => buffer[index] === byte)) throw new Error('could not decode image as PNG.');
  } else if (mimeType === 'image/jpeg') {
    const jpegMagicNumber = [255, 216];
    if (buffer.length < jpegMagicNumber.length || !jpegMagicNumber.every((byte, index) => buffer[index] === byte)) throw new Error('could not decode image as JPEG.');
  }
}
function compareText(actual, expectedBuffer) {
  if (typeof actual !== 'string') return {
    errorMessage: 'Actual result should be a string'
  };
  let expected = expectedBuffer.toString('utf-8');
  if (expected === actual) return null;
  // Eliminate '\\ No newline at end of file'
  if (!actual.endsWith('\n')) actual += '\n';
  if (!expected.endsWith('\n')) expected += '\n';
  const lines = _utilsBundle.diff.createPatch('file', expected, actual, undefined, undefined, {
    context: 5
  }).split('\n');
  const coloredLines = lines.slice(4).map(line => {
    if (line.startsWith('-')) return _utilsBundle.colors.red(line);
    if (line.startsWith('+')) return _utilsBundle.colors.green(line);
    if (line.startsWith('@@')) return _utilsBundle.colors.dim(line);
    return line;
  });
  const errorMessage = coloredLines.join('\n');
  return {
    errorMessage
  };
}
function resizeImage(image, size) {
  if (image.width === size.width && image.height === size.height) return image;
  const buffer = new Uint8Array(size.width * size.height * 4);
  for (let y = 0; y < size.height; y++) {
    for (let x = 0; x < size.width; x++) {
      const to = (y * size.width + x) * 4;
      if (y < image.height && x < image.width) {
        const from = (y * image.width + x) * 4;
        buffer[to] = image.data[from];
        buffer[to + 1] = image.data[from + 1];
        buffer[to + 2] = image.data[from + 2];
        buffer[to + 3] = image.data[from + 3];
      } else {
        buffer[to] = 0;
        buffer[to + 1] = 0;
        buffer[to + 2] = 0;
        buffer[to + 3] = 0;
      }
    }
  }
  return {
    data: Buffer.from(buffer),
    width: size.width,
    height: size.height
  };
}