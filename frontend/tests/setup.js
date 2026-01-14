import { afterAll, vi, beforeAll } from 'vitest';

global.jest = vi;

global.window.resizeTo = (width, height) => {
  global.window.innerWidth = width || global.window.innerWidth;
  global.window.innerHeight = height || global.window.innerHeight;
  global.window.dispatchEvent(new Event('resize', { bubbles: true, cancelable: true }));
};

beforeAll(() => {});

afterAll(() => {
  delete global.jest;
  delete global.window.jest;
});
