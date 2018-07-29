using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace SortingAlgorithmsUnitTests
{
    [TestFixture]
    public class HeapSortTests
    {
        private readonly int[] _unsortedDataSet = { 1, 6, 3, 2, 8, 5, 9 };
        [Test]
        public void HeapSort_sorts_data_set_when_called()
        {
            //var dataset = _unsortedDataSet.ToList();

            //dataset = dataset.BuildBinarySearchTree().ToList();
            //var systemSortedDataSet = dataset.OrderBy(item => item).ToList();
            //Assert.That(dataset, Is.EqualTo(systemSortedDataSet));
        }

        [Test]
        public void BuildBinaryTree_discovery_test()
        {
            var binaryTree = _unsortedDataSet.BuildBinarySearchTree();

            Console.WriteLine(JsonConvert.SerializeObject(binaryTree));
            Assert.That(true);
        }

        [Test]
        public void OrderByBinarySearchTreeAscending_orders_data_set_when_called()
        {
            var dataset = _unsortedDataSet.ToList();

            var sortedDataSet = dataset.OrderByBinarySearchTreeAscending();
            Console.WriteLine(JsonConvert.SerializeObject(sortedDataSet));
            var systemSortedDataSet = dataset.OrderBy(item => item).ToList();
            Assert.That(sortedDataSet, Is.EqualTo(systemSortedDataSet));
        }

        [Test]
        public void OrderByBinarySearchTreeAscending_orders_large_data_set_when_called()
        {
            var dataset = GetRandomNumberSet(100);

            var systemSortedDataSet = dataset.OrderBy(item => item).ToList();

            var sortedDataSet = dataset.OrderByBinarySearchTreeAscending();
            Console.WriteLine(JsonConvert.SerializeObject(sortedDataSet));

            Assert.That(sortedDataSet, Is.EqualTo(systemSortedDataSet));
        }

        private int[] GetRandomNumberSet(int sizeOfSet)
        {
            var numbers = new int[sizeOfSet];
            for (var i = 0; i < sizeOfSet; i++)
            {
                numbers[i] = Math.Abs(Guid.NewGuid().GetHashCode());
            }

            return numbers;
        }

    }

    public static class SortingExtensions
    {
        public static int[] OrderByBinarySearchTreeAscending(this IList<int> dataset)
        {
            var binarySearchTree = dataset.BuildBinarySearchTree();
            var orderedDataSet = new int[dataset.Count];
            PopTree(binarySearchTree, 0, orderedDataSet, 0);
            return orderedDataSet;
        }

        public static SortableNode[] BuildBinarySearchTree(this IList<int> dataset)
        {
            var binaryTree = new SortableNode[dataset.Count];
            binaryTree[0] = new SortableNode(dataset[0]);
            return PushToBinaryTree(0, 1, dataset.ToArray(), binaryTree);
        }

        private static SortableNode[] PushToBinaryTree(int rootNodePosition, int nextIndex, int[] dataset, SortableNode[] binaryTree)
        {
            
            if (nextIndex >= dataset.Length)
                return binaryTree;

            var rootNode = binaryTree[rootNodePosition];
            var nextNodeValue = dataset[nextIndex];

            if (nextNodeValue <= rootNode.Value)
            {
                if (!rootNode.LeftPosition.HasValue)
                {
                    var nextNode = new SortableNode(nextNodeValue)
                    {
                        ParentPosition = rootNodePosition
                    };
                    binaryTree[nextIndex] = nextNode;
                    rootNode.LeftPosition = nextIndex;
                }
                else
                    PushToBinaryTree(rootNode.LeftPosition.Value, nextIndex, dataset, binaryTree);
            }
            else
            {
                if (!rootNode.RightPosition.HasValue)
                {
                    var nextNode = new SortableNode(nextNodeValue)
                    {
                        ParentPosition = rootNodePosition
                    };
                    binaryTree[nextIndex] = nextNode;
                    rootNode.RightPosition = nextIndex;
                } 
                else
                    PushToBinaryTree(rootNode.RightPosition.Value, nextIndex, dataset, binaryTree);
            }

            if (rootNode.Value == dataset[0])
                return PushToBinaryTree(rootNodePosition, nextIndex + 1, dataset, binaryTree);

            return binaryTree;
        }

        private static int PopTree(SortableNode[] binaryTree, int positionInTree, int[] orderedData, int positionInOrderedData)
        {
            //if parent is null
            //if node is empty, pop node, return binary tree
            //else if left is empty, pop node, dissociate parent from right child, right child now root
            // else attempt to pop left child
            var currentNode = binaryTree[positionInTree];
            if (!currentNode.ParentPosition.HasValue)
            {
                if (!currentNode.LeftPosition.HasValue && !currentNode.RightPosition.HasValue)
                {
                    SetNextOrderedValue(orderedData, positionInOrderedData, currentNode);
                    return positionInOrderedData;
                }

                if (!currentNode.LeftPosition.HasValue)
                {
                    SetNextOrderedValue(orderedData, positionInOrderedData, currentNode);
                    int rightPosition = DissociateRightChild(binaryTree, currentNode);
                    return PopTree(binaryTree, rightPosition, orderedData, positionInOrderedData + 1);
                }
            }
            else
            {
                //if parent is not null
                //if node is empty, dissociate from parent, add to list
                //else if left is empty, parent.left == currentnode.right, add currentNode to list
                //else attempt to pop left child
                if (!currentNode.LeftPosition.HasValue && !currentNode.RightPosition.HasValue)
                {
                    SetNextOrderedValue(orderedData, positionInOrderedData, currentNode);
                    return positionInOrderedData;
                }

                if (!currentNode.LeftPosition.HasValue)
                {
                    SetNextOrderedValue(orderedData, positionInOrderedData, currentNode);
                    binaryTree[currentNode.ParentPosition.Value].LeftPosition = currentNode.RightPosition.Value;
                    return PopTree(binaryTree, currentNode.ParentPosition.Value, orderedData, positionInOrderedData + 1);
                }
            }
            
            var lastFilledPosition = PopTree(binaryTree, currentNode.LeftPosition.Value, orderedData, positionInOrderedData);
            if (currentNode.IsNodeEmpty)
            {
                return lastFilledPosition;
            }
            DissociateLeftChild(binaryTree, currentNode);
            return PopTree(binaryTree, positionInTree, orderedData, lastFilledPosition + 1);
        }

        private static void SetNextOrderedValue(int[] orderedData, int positionInOrderedData, SortableNode currentNode)
        {
            orderedData[positionInOrderedData] = currentNode.Value;
        }

        private static int DissociateRightChild(SortableNode[] binaryTree, SortableNode currentNode)
        {
            var rightPosition = currentNode.RightPosition.Value;
            binaryTree[rightPosition].ParentPosition = null;
            currentNode.RightPosition = null;
            return rightPosition;
        }

        private static int DissociateLeftChild(SortableNode[] binaryTree, SortableNode currentNode)
        {
            var leftPosition = currentNode.LeftPosition.Value;
            currentNode.LeftPosition = null;
            return leftPosition;
        }
    }


    public class SortableNode
    {
        public SortableNode(int value)
        {
            Value = value;
        }

        public int Value { get; }
        public int? LeftPosition { get; set; }
        public int? RightPosition { get; set; }
        public int? ParentPosition { get; set; }
        public bool IsLeftEmpty => !LeftPosition.HasValue;
        public bool IsRightEmpty => !RightPosition.HasValue;
        public bool IsRoot => !ParentPosition.HasValue;
        public bool IsNodeEmpty => IsLeftEmpty && IsRightEmpty;
    }
}
