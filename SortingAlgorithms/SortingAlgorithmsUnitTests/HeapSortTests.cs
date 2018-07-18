using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SortingAlgorithmsUnitTests
{
    [TestFixture]
    public class HeapSortTests
    {
        [Test]
        public void HeapSort_sorts_data_set_when_called()
        {
            var dataset = new[] {1, 6, 3, 2, 8, 5, 9};
            dataset = dataset.OrderBy(dataitem => dataitem).ToArray();
            var systemSortedDataSet = dataset.OrderBy(dataitem => dataitem).ToList();

            Assert.That(dataset, Is.EqualTo(systemSortedDataSet));
        }
    }
}
