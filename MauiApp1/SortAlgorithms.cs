using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    internal static class SortAlgorithms
    {
        public static (List<int> sortedArray, int comparisons) BubbleSort(int[] array)
        {
            int n = array.Length;
            List<int> sortedArray = new List<int>(array);
            int comparisons = 0;

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    comparisons++;
                    if (sortedArray[j] > sortedArray[j + 1])
                    {
                        // Swap
                        int temp = sortedArray[j];
                        sortedArray[j] = sortedArray[j + 1];
                        sortedArray[j + 1] = temp;
                    }
                }
            }

            return (sortedArray, comparisons);
        }

        public static (List<int> sortedArray, int comparisons) MergeSort(int[] array)
        {
            return MergeSortHelper(array, 0, array.Length - 1);
        }

        private static (List<int> sortedArray, int comparisons) MergeSortHelper(int[] array, int left, int right)
        {
            int comparisons = 0;
            if (left < right)
            {
                int mid = (left + right) / 2;
                var leftResult = MergeSortHelper(array, left, mid);
                var rightResult = MergeSortHelper(array, mid + 1, right);
                comparisons += leftResult.comparisons + rightResult.comparisons;

                var mergedResult = Merge(leftResult.sortedArray, rightResult.sortedArray);
                comparisons += mergedResult.comparisons;
                return (mergedResult.sortedArray, comparisons);
            }

            return (new List<int> { array[left] }, comparisons);
        }

        private static (List<int> sortedArray, int comparisons) Merge(List<int> left, List<int> right)
        {
            List<int> result = new List<int>();
            int comparisons = 0;
            int i = 0, j = 0;

            while (i < left.Count && j < right.Count)
            {
                comparisons++;
                if (left[i] <= right[j])
                {
                    result.Add(left[i]);
                    i++;
                }
                else
                {
                    result.Add(right[j]);
                    j++;
                }
            }

            // Add remaining elements
            result.AddRange(left.GetRange(i, left.Count - i));
            result.AddRange(right.GetRange(j, right.Count - j));

            return (result, comparisons);
        }

        public static (List<int> sortedArray, int comparisons) HeapSort(int[] array)
        {
            int n = array.Length;
            List<int> sortedArray = new List<int>(array);
            int comparisons = 0;

            // Build heap (rearrange array)
            for (int i = n / 2 - 1; i >= 0; i--)
                Heapify(sortedArray, n, i, ref comparisons);

            // One by one extract an element from heap
            for (int i = n - 1; i > 0; i--)
            {
                // Move current root to end
                int temp = sortedArray[0];
                sortedArray[0] = sortedArray[i];
                sortedArray[i] = temp;

                // call max heapify on the reduced heap
                Heapify(sortedArray, i, 0, ref comparisons);
            }

            return (sortedArray, comparisons);
        }

        private static void Heapify(List<int> array, int n, int i, ref int comparisons)
        {
            int largest = i; // Initialize largest as root
            int left = 2 * i + 1; // left = 2*i + 1
            int right = 2 * i + 2; // right = 2*i + 2

            if (left < n)
            {
                comparisons++;
                if (array[left] > array[largest])
                    largest = left;
            }

            if (right < n)
            {
                comparisons++;
                if (array[right] > array[largest])
                    largest = right;
            }

            // If largest is not root
            if (largest != i)
            {
                int swap = array[i];
                array[i] = array[largest];
                array[largest] = swap;

                // Recursively heapify the affected sub-tree
                Heapify(array, n, largest, ref comparisons);
            }
        }

        public static (List<int> sortedArray, int comparisons) QuickSort(int[] array)
        {
            List<int> sortedArray = new List<int>(array);
            int comparisons = QuickSortHelper(sortedArray, 0, sortedArray.Count - 1);
            return (sortedArray, comparisons);
        }

        private static int QuickSortHelper(List<int> array, int low, int high)
        {
            int comparisons = 0;
            if (low < high)
            {
                int pi = Partition(array, low, high, ref comparisons);

                comparisons += QuickSortHelper(array, low, pi - 1);
                comparisons += QuickSortHelper(array, pi + 1, high);
            }
            return comparisons;
        }

        private static int Partition(List<int> array, int low, int high, ref int comparisons)
        {
            int pivot = array[high];
            int i = (low - 1); // Index of smaller element

            for (int j = low; j < high; j++)
            {
                comparisons++;
                if (array[j] <= pivot)
                {
                    i++;
                    // Swap
                    int temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }

            // Swap the pivot element with the element at i + 1
            int temp1 = array[i + 1];
            array[i + 1] = array[high];
            array[high] = temp1;

            return i + 1;
        }

        public static (List<int> sortedArray, int comparisons) RadixSort(int[] array)
        {
            int max = array.Max();
            int comparisons = 0;

            for (int exp = 1; max / exp > 0; exp *= 10)
            {
                var (sortedArray, iterationCount) = CountingSort(array, exp);
                comparisons += iterationCount;
                array = sortedArray.ToArray();
            }

            return (new List<int>(array), comparisons);
        }

        private static (List<int> sortedArray, int comparisons) CountingSort(int[] array, int exp)
        {
            int n = array.Length;
            int[] output = new int[n]; // output array
            int count = 10; // Range of digits (0-9)
            int[] countArray = new int[count];

            // Store count of occurrences of each digit
            for (int i = 0; i < n; i++)
            {
                int index = (array[i] / exp) % 10;
                countArray[index]++;
            }

            // Update count[i] to be the actual position of this digit in output[]
            for (int i = 1; i < count; i++)
            {
                countArray[i] += countArray[i - 1];
            }

            // Build the output array
            for (int i = n - 1; i >= 0; i--)
            {
                int index = (array[i] / exp) % 10;
                output[countArray[index] - 1] = array[i];
                countArray[index]--;
            }

            return (output.ToList(), n); // Return sorted array and number of iterations
        }
    }
}
