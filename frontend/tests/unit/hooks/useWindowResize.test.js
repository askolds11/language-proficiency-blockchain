import { expect, test, describe } from 'vitest';
import useWindowResize from '@/hooks/useWindowResize';
import { mount } from '@vue/test-utils';

const TestComponent = {
  template: '<div></div>',
  setup() {
    const { width, height } = useWindowResize();
    return { width, height };
  },
};

describe('Test resize hook', () => {
  test('is resize props changing', async () => {
    const wrapper = mount(TestComponent);
    const { vm } = wrapper;
    global.resizeTo(100, 200);
    expect(vm.width).toBe(100);
    expect(vm.height).toBe(200);
    global.resizeTo(1920, 1080);
    expect(vm.width).toBe(1920);
    expect(vm.height).toBe(1080);
  });
});
