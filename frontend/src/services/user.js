import mocksProd from '@/services/mocksProd';

export async function getUserSettings() {
  // wait 4 seconds to simulate slow response
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve({
        status: 200,
        data: {
          savedValues: {
            autocompletePreselected: mocksProd.autocompletePreselected,
            autocompletePreselectedFunc: mocksProd.autocompletePreselectedFunc,
          },
        },
      });
    }, 1000);
  });
}
