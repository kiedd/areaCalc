using System.Runtime.InteropServices.WindowsRuntime;
using System.Reflection;
using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using System.Collections.Generic;

namespace mediancalc
{
    class Program
    {
        private static IFixture fixture = new Fixture();
        static void Main(string[] args)
        {
            GetExpectedMedian(new[] { 1, 3 }, new[] { 2 }).Should().Be(2.0);
            GetExpectedMedian(new[] { 1, 2 }, new[] { 3, 4 }).Should().Be(2.5);
            GetExpectedMedian(new[] { 1, 2, 3, 4 }, new[] { 2, 4, 6, 8, 10 }).Should().Be(4);
            // FindMedianSortedArrays(new[] { 1, 3 }, new[] { 2 }).Should().Be(2.0);
            // FindMedianSortedArrays(new[] { 1, 2 }, new[] { 3, 4 }).Should().Be(2.5);
            // FindMedianSortedArrays(new[] { 1, 2 }, new int[] { }).Should().Be(1.5);
            // FindMedianSortedArrays(new int[] { }, new[] { 1, 2 }).Should().Be(1.5);
            // FindMedianSortedArrays(new int[] { 10000 }, new[] { 10001 }).Should().Be(10000.5);
            //FindMedianSortedArrays(new[] { 1, 2, 3, 4 }, new[] { 2, 4, 6, 8, 10 }).Should().Be(4);
            FindMedianSortedArrays(new int[] { 1, 2 }, new[] { -1, 3 }).Should().Be(1.5);
            return;
            for (int i = 0; i < 10000; i++)
            {
                var nums1 = CreateSortedArray();
                var nums2 = CreateSortedArray();

                var expected = GetExpectedMedian(nums1, nums2);
                var actual = FindMedianSortedArrays(nums1, nums2);
                actual.Should().Be(expected);
            }
        }

        private static double GetExpectedMedian(int[] nums1, int[] nums2)
        {
            var union = nums1.Concat(nums2).OrderBy(x => x).ToList();
            if (union.Count % 2 == 0)
            {
                return (double)(union[union.Count / 2] + union[(union.Count / 2) - 1]) / 2;
            }
            else
            {
                return union[(union.Count - 1) / 2];
            }
        }

        private static int[] CreateSortedArray()
        {
            return fixture.CreateMany<int>(10).OrderBy(x => x).Take(10).ToArray();
        }

        private static double FindMedianSortedArrays(int[] nums1, int[] nums2)
        {
            var result = ThrowOutElements(nums1, nums2);
            if (result.Median.HasValue)
            {
                return result.Median.Value;
            }

            if (!result.nums1.Any())
            {
                return GetMedian(result.nums2).Item2;
            }

            if (!result.nums2.Any())
            {
                return GetMedian(result.nums1).Item2;
            }

            if (result.right > result.left)
            {
                return Math.Max(result.nums1.Single(), result.nums2.Single());
            }

            if (result.right < result.left)
            {
                return Math.Min(result.nums1.Single(), result.nums2.Single());
            }

            return (double)(result.nums1.Single() + result.nums2.Single()) / 2;
        }

        private static ThrowOutElementsResult ThrowOutElements(IEnumerable<int> nums1, IEnumerable<int> nums2, int left = 0, int right = 0)
        {
            var retVal = new ThrowOutElementsResult();
            var cnt1 = nums1.Count();
            var cnt2 = nums2.Count();
            if (cnt1 == 0 || cnt2 == 0 || (cnt1 == 1 && cnt2 == 1))
            {
                retVal.nums1 = nums1;
                retVal.nums2 = nums2;
                return retVal;
            }
            var median1 = GetMedian(nums1);
            var median2 = GetMedian(nums2);
            if (median1.Item2 == median2.Item2)
            {
                retVal.Median = median1.Item2;
                return retVal;
            }

            if (median1.Item2 > median2.Item2)
            {

                retVal.left = (int)median2.Item1 + 1;
                retVal.right = (int)((double)(nums1.Count()) - median1.Item1);

                retVal.nums1 = nums1.Take(nums1.Count() - retVal.right);
                retVal.nums2 = nums2.Skip(retVal.left);
            }
            else
            {
                retVal.left = (int)median1.Item1 + 1;
                retVal.right = (int)((double)(nums2.Count()) - median2.Item1);

                retVal.nums1 = nums1.Skip(retVal.left);
                retVal.nums2 = nums2.Take(nums2.Count() - retVal.right);
            }

            retVal.left += left;
            retVal.right += right;

            cnt1 = retVal.nums1.Count();
            cnt2 = retVal.nums2.Count();
            if (cnt1 == 0 || cnt2 == 0 || (cnt1 == 1 && cnt2 == 1))
            {
                return retVal;
            }

            return ThrowOutElements(retVal.nums1, retVal.nums2, retVal.left, retVal.right);
        }

        private static Tuple<double, double> GetMedian(IEnumerable<int> nums)
        {
            int count = nums.Count();
            if (count % 2 == 0)
            {
                return Tuple.Create((double)(count - 1) / 2.0, (double)(nums.ElementAt(count / 2) + nums.ElementAt((count / 2) - 1)) / 2);
            }
            else
            {
                return Tuple.Create((double)(count - 1) / 2.0, (double)nums.ElementAt((count - 1) / 2));
            }
        }

        private class ThrowOutElementsResult
        {
            public double? Median { get; set; }

            public int left { get; set; }

            public int right { get; set; }

            public IEnumerable<int> nums1 { get; set; }

            public IEnumerable<int> nums2 { get; set; }
        }
    }
}
