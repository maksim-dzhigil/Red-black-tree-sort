using System.Text;
using System.Xml;

namespace LW_RBTreeSort
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("All tests passed: {0}", Test());
            Console.WriteLine("View experiments result?(yes\\no)");
            string userAnswer = Console.ReadLine();
            var showResults = userAnswer == "yes";
            Console.WriteLine("The program has started working");
            GetExperimentResults(showResults);
            Console.WriteLine("The program has finished working. You can see results in \"report.csv\"");
            Console.WriteLine("Sort your array?(yes/no)");
            userAnswer = Console.ReadLine();
            var sortUserArray = userAnswer == "yes";
            if (sortUserArray)
            {
                Console.WriteLine("Enter the type(string/int)");
                userAnswer = Console.ReadLine();
                var typeOfElements = userAnswer == "int" ? "int" : userAnswer == "double" ? "double" : "string";
                Console.WriteLine("Enter array elements separated by a space");
                string arrayToSort = Console.ReadLine();
                switch (typeOfElements)
                {
                    case "int":
                    {
                        int[] intArray = arrayToSort.Split(' ').Select(x => int.Parse(x)).ToArray();
                        var sortedArray = new RBTree<int>(intArray).sortedArray;
                        Console.WriteLine("Sorted array:");
                        foreach (var element in sortedArray)
                        {
                            Console.Write("{0} ", element);
                        }
                        
                        break;
                    }
                    case "string":
                    {
                        string[] stringArray = arrayToSort.Split(' ').Select(x => x.ToString()).ToArray();
                        var sortedArray = new RBTree<string>(stringArray).sortedArray;
                        Console.WriteLine("Sorted array:");
                        foreach (var element in sortedArray)
                        {
                            Console.Write("{0} ", element);
                        }
                        
                        break;
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine("Thank you for your attention! Press any key to finish");
            Console.ReadKey();
        }
        private static bool Test()
        {
            var isAllTestsCompleted = true;
            {
                var startarray = new[] {10, 9, 8, 7, 6, 5, 4, 3, 2, 1};
                var expectedarray = new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
                var sortedatarray = new RBTree<int>(startarray).sortedArray;
                
                if (!Enumerable.SequenceEqual(sortedatarray, expectedarray))
                {
                    Console.WriteLine("Test1 wasn't passed");
                    isAllTestsCompleted = false;
                }
            }
            {
                var startarray = new[] {232, 89, 400, 121, 83, 340};
                var expectedarray = new[] {83, 89, 121, 232, 340, 400};
                var sortedatarray = new RBTree<int>(startarray).sortedArray;

                if (!Enumerable.SequenceEqual(sortedatarray, expectedarray))
                {
                    Console.WriteLine("Test2 wasn't passed");
                    isAllTestsCompleted = false;
                }
            }
            {
                var startarray = new[] {"bdg", "avc", "hdf", "dg", "aa", "chd"};
                var expectedarray = new[] {"aa", "avc", "bdg", "chd", "dg", "hdf"};
                var sortedatarray = new RBTree<string>(startarray).sortedArray;

                if (!Enumerable.SequenceEqual(sortedatarray, expectedarray))
                {
                    Console.WriteLine("Test1 wasn't passed");
                    isAllTestsCompleted = false;
                }
            }
            return isAllTestsCompleted;
        }
        private static Tuple<int, StringBuilder> GetOperationsCount(int minElement, int maxElement, int repeatsCount, int arrayLength)
        {
            var rowResults = new StringBuilder();
            var operationsCount = 0;
            for (var i = 0; i < repeatsCount; i++)
            {
                var array = new int[arrayLength];
                var random = new Random();
                for (var j = 0; j < array.Length; j++)
                    array[j] = random.Next(minElement, maxElement);
                var tree = new RBTree<int>(array);
                operationsCount += tree.operationsCount;
                rowResults.AppendLine(string.Format("{0}, {1}", arrayLength, tree.operationsCount));
            }

            return new Tuple<int, StringBuilder>(operationsCount, rowResults);
        }
        
        private static void GetExperimentResults(bool showResults)
        {
            var xDoc = new XmlDocument();
            var out_csv = new StringBuilder();
            out_csv.AppendLine("");
            xDoc.Load(Path.Combine(Environment.CurrentDirectory, "Experiment.xml"));
            var xRoot = xDoc.DocumentElement;
            var experiment = xRoot.SelectSingleNode("experiment");
            foreach (XmlNode node in experiment.ChildNodes)
            {
                var minElement =
                    int.Parse(node.SelectSingleNode("@minElement").Value); 
                var maxElement = int.Parse(node.SelectSingleNode("@maxElement").Value);
                var startLength = int.Parse(node.SelectSingleNode("@startLength").Value);
                var maxLength = int.Parse(node.SelectSingleNode("@maxLength").Value);
                var repeat = int.Parse(node.SelectSingleNode("@repeat").Value);
                var name = node.SelectSingleNode("@name").Value;
                
                if (name == "Arithmetic Progression")
                {
                    var diff = int.Parse(node.SelectSingleNode("@diff").Value);
                    if (showResults) 
                        Console.WriteLine("Эксперимент: " + name);
                    for (var length = startLength; length <= maxLength; length += diff)
                    {
                        var getOperTuple = GetOperationsCount(minElement, maxElement, repeat, length);
                        var compareCount = getOperTuple.Item1 / repeat;
                        out_csv.Append(getOperTuple.Item2);
                        if (showResults)
                            Console.WriteLine("Длина массива: " + length + "\t" + "Кол-вомассивов: " + repeat + "\t" +
                                          "Операций в среднем: " + compareCount);
                    }
                }

                if (name == "Geometric Progression")
                {
                    var znamen = double.Parse(node.SelectSingleNode("@Znamen").Value);
                    if (showResults)
                        Console.WriteLine("Эксперимент: " + name);
                    for (var length = startLength; length <= maxLength; length = (int)Math.Round(length * znamen))
                    {
                        var getOPerTuple = GetOperationsCount(minElement, maxElement, repeat, length);
                        var compareCount = getOPerTuple.Item1 / repeat;
                        out_csv.Append(getOPerTuple.Item2);
                        if (showResults)
                            Console.WriteLine("Длина массива: " + length + "\t" + "Кол-вомассивов: " + repeat + "\t" +
                                          "Операций в среднем: " + compareCount);
                    }
                }

                File.AppendAllText(Path.Combine(Environment.CurrentDirectory, "report.csv"), out_csv.ToString());
                if (showResults)
                    Console.WriteLine("\n");
                else
                    Console.WriteLine("Still working...");
            }
        }
    }
    enum Color
    {
        Red,
        Black
    }
    class RBTree<T> where T : IComparable
    {
        public class RBTNode
        {
            public Color Сolour;
            public RBTNode Left;
            public RBTNode Right;
            public RBTNode Parent;
            public T Data;
 
            public RBTNode(T data) { Data = data; }
            public RBTNode(Color colour) { Сolour = colour; }
            public RBTNode(T data, Color colour) { Data = data; Сolour = colour; }
        }
        
        private RBTNode rootNode;
        public int operationsCount;
        public List<T> sortedArray;

        public RBTree(IEnumerable<T> array)
        {
            rootNode = null;
            operationsCount = 0;
            sortedArray = new List<T>();
            
            foreach (var item in array)
                Insert(item);
            Sort(rootNode);
        }

        private void LeftRotate(RBTNode currentNode)
        {
            RBTNode replacedNode = currentNode.Right;
            currentNode.Right = replacedNode.Left;
            if (replacedNode.Left != null)
            {
                replacedNode.Left.Parent = currentNode;
                operationsCount++;
            }

            if (replacedNode != null)
            {
                replacedNode.Parent = currentNode.Parent;
                operationsCount++;
            }

            if (currentNode.Parent == null)
            {
                rootNode = replacedNode;
                operationsCount++;
            }

            else if (currentNode == currentNode.Parent.Left)
            {
                currentNode.Parent.Left = replacedNode;
                operationsCount++;
            }
            else
            {
                currentNode.Parent.Right = replacedNode;
            }

            replacedNode.Left = currentNode;
            if (currentNode != null)
            {
                currentNode.Parent = replacedNode;
                operationsCount++;
            }
        }

        private void RightRotate(RBTNode currentNode)
        {
            RBTNode replacedNode = currentNode.Left;
            currentNode.Left = replacedNode.Right;
            if (replacedNode.Right != null)
            {
                replacedNode.Right.Parent = currentNode;
                operationsCount++;
            }
            if (replacedNode != null)
            {
                replacedNode.Parent = currentNode.Parent;
                operationsCount++;
            }
            if (currentNode.Parent == null)
            {
                rootNode = replacedNode;
                operationsCount++;
            }
            else if (currentNode == currentNode.Parent.Right)
            {
                 currentNode.Parent.Right = replacedNode;
                 operationsCount++;
            }
            else
            {
                 currentNode.Parent.Left = replacedNode;
                 operationsCount++;
            }
 
            replacedNode.Right = currentNode;
            if (currentNode != null)
            {
                currentNode.Parent = replacedNode;
                operationsCount++;
            }
        }

        public void Insert(T item)
        {
            RBTNode newItem = new RBTNode(item);
            if (rootNode == null)
            {
                rootNode = newItem;
                rootNode.Сolour = Color.Black;
                operationsCount++;
                return;
            }
            RBTNode parentNode = null;
            RBTNode brotherNode = rootNode;
            while (brotherNode != null)
            {
                parentNode = brotherNode;
                if (newItem.Data.CompareTo(brotherNode.Data) <= 0)
                {
                    brotherNode = brotherNode.Left;
                    operationsCount++;
                }
                else
                {
                    brotherNode = brotherNode.Right;
                    operationsCount++;
                }
            }

            newItem.Parent = parentNode;
            if (newItem.Data.CompareTo(parentNode.Data) <= 0)
            {
                parentNode.Left = newItem;
                operationsCount++;
            }
            else
            {
                parentNode.Right = newItem;
            }
            newItem.Left = null;
            newItem.Right = null;
            newItem.Сolour = Color.Red;
            CorrectionAfterInsert(newItem);
        }
        
        private void Sort(RBTNode current)
        {
            if (current.Left != null)
            {
                Sort(current.Left);
            }
            sortedArray.Add(current.Data);
            if (current.Right != null)
            {
                Sort(current.Right);
            }
        }
        private void CorrectionAfterInsert(RBTNode item)
        {
            while (item != rootNode && item.Parent.Сolour == Color.Red)
            {
                if (item.Parent == item.Parent.Parent.Left)
                {
                    operationsCount++;
                    RBTNode rightUncleNode = item.Parent.Parent.Right;
                    if (rightUncleNode != null && rightUncleNode.Сolour == Color.Red)
                    {
                        operationsCount++;
                        item.Parent.Сolour = Color.Black;
                        rightUncleNode.Сolour = Color.Black;
                        item.Parent.Parent.Сolour = Color.Red;
                        item = item.Parent.Parent;
                    }
                    else
                    {
                        if (item == item.Parent.Right)
                        {
                            operationsCount++;
                            item = item.Parent;
                            LeftRotate(item);
                        }
                    item.Parent.Сolour = Color.Black;
                    item.Parent.Parent.Сolour = Color.Red;
                    RightRotate(item.Parent.Parent);
                    }
                }
                else
                {
                    RBTNode leftUncleNode = item.Parent.Parent.Left;
                    if (leftUncleNode != null && leftUncleNode.Сolour == Color.Black)
                    { 
                        operationsCount++;
                        item.Parent.Сolour = Color.Red;
                        leftUncleNode.Сolour = Color.Red;
                        item.Parent.Parent.Сolour = Color.Black;
                        item = item.Parent.Parent;
                    }
                    else
                    {
                        if (item == item.Parent.Left)
                        {
                            operationsCount++;
                            item = item.Parent;
                            RightRotate(item);
                        }
                    item.Parent.Сolour = Color.Black;
                    item.Parent.Parent.Сolour = Color.Red;
                    LeftRotate(item.Parent.Parent);
                    }
                }
                rootNode.Сolour = Color.Black;
            }
        }
    }
}