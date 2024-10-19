namespace ReportGenerator
{
    class QuarterlyIncomeReport
    {
        static void Main(string[] args)
        {
            // create a new instance of the class
            QuarterlyIncomeReport report = new QuarterlyIncomeReport();

            // call the GenerateSalesData method
            SalesData[] salesData = report.GenerateSalesData();


            // call the QuarterlySalesReport method
            report.QuarterlySalesReport(salesData);

        }

        /* public struct SalesData includes the following fields: date sold, department name, product ID, quantity sold, unit price */
        public struct SalesData
        {
            public DateOnly dateSold;
            public string departmentName;
            public string productID;
            public int quantitySold;
            public double unitPrice;
            public double baseCost;
            public int volumeDiscount;
        }

        public struct ProdDepartments
        {
            public static string[] departmentNames = { "Men's Clothing", "Women's Clothing", "Children's Clothing", "Accessories", "Footwear", "Outerwear", "Sportswear", "Undergarments" };
            public static string[] departmentAbbreviations = { "MENS", "WOMN", "CHLD", "ACCS", "FOOT", "OUTR", "SPRT", "UNDR" };
        }

        public struct ManufacturingSites
        {
            public static string[] manufacturingSites = { "US1", "US2", "US3", "UK1", "UK2", "UK3", "JP1", "JP2", "JP3", "CA1" };
        }

        /* the GenerateSalesData method returns 1000 SalesData records. It assigns random values to each field of the data structure */
        public SalesData[] GenerateSalesData()
        {
            SalesData[] salesData = new SalesData[1000];
            Random random = new Random();

            for (int i = 0; i < 1000; i++)
            {
                salesData[i].dateSold = new DateOnly(2023, random.Next(1, 13), random.Next(1, 29));
                salesData[i].departmentName = ProdDepartments.departmentNames[random.Next(0, ProdDepartments.departmentNames.Length)];

                salesData[i].productID = ConstructProductId(salesData[i].departmentName, random);

                salesData[i].quantitySold = random.Next(1, 101);
                salesData[i].unitPrice = random.Next(25, 300) + random.NextDouble();
                salesData[i].baseCost = salesData[i].unitPrice * (1 - (random.Next(5, 21) / 100.0));
                salesData[i].volumeDiscount = (int)(salesData[i].quantitySold * 0.1);

            }

            return salesData;
        }
        private string ConstructProductId(string departmentName, Random random)
        {
            string deptCode = departmentName.Substring(0, 4).ToUpper();
            string productNumber = random.Next(1, 1000).ToString("D3");
            string sizeCode = new string[] { "XS", "S", "M", "L", "XL" }[random.Next(0, 5)];
            string colorCode = new string[] { "BK", "BL", "GR", "RD", "YL", "OR", "WT", "GY" }[random.Next(0, 8)];
            string manufacturingSite = ManufacturingSites.manufacturingSites[random.Next(0, ManufacturingSites.manufacturingSites.Length)];

            return $"{deptCode}-{productNumber}-{sizeCode}-{colorCode}-{manufacturingSite}";
        }

        private string[,] DeconstructProductId(string productId)
        {
            string[] parts = productId.Split('-');
            string[,] components = new string[5, 2];

            components[0, 0] = "Department Code";
            components[0, 1] = parts[0];

            components[1, 0] = "Product Number";
            components[1, 1] = parts[1];

            components[2, 0] = "Size Code";
            components[2, 1] = parts[2];

            components[3, 0] = "Color Code";
            components[3, 1] = parts[3];

            components[4, 0] = "Manufacturing Site";
            components[4, 1] = parts[4];

            return components;
        }

        private string GenerateProductSerialNumber(string productId)
        {
            string[,] components = DeconstructProductId(productId);
            string departmentCode = components[0, 1];
            string productNumber = components[1, 1];
            return $"{departmentCode}-{productNumber}-ss-cc-mmm";
        }
        public void QuarterlySalesReport(SalesData[] salesData)
        {
            var quarterlySales = new Dictionary<string, double>();
            var quarterlyProfit = new Dictionary<string, double>();
            var quarterlyProfitPercentage = new Dictionary<string, double>();

            var quarterlySalesByDepartment = new Dictionary<string, Dictionary<string, double>>();
            var quarterlyProfitByDepartment = new Dictionary<string, Dictionary<string, double>>();
            var quarterlyProfitPercentageByDepartment = new Dictionary<string, Dictionary<string, double>>();

            var top3SalesOrdersByQuarter = new Dictionary<string, List<SalesData>>();
            var productsSoldByQuarter = new Dictionary<string, List<string>>();

            foreach (var data in salesData)
            {
            var quarter = GetQuarter(data.dateSold.Month);
            var totalSales = data.quantitySold * data.unitPrice;
            var totalCost = data.quantitySold * data.baseCost;
            var profit = totalSales - totalCost;
            var profitPercentage = (profit / totalSales) * 100;

            if (!quarterlySalesByDepartment.ContainsKey(quarter))
            {
                quarterlySalesByDepartment[quarter] = new Dictionary<string, double>();
                quarterlyProfitByDepartment[quarter] = new Dictionary<string, double>();
                quarterlyProfitPercentageByDepartment[quarter] = new Dictionary<string, double>();
            }

            if (quarterlySalesByDepartment[quarter].ContainsKey(data.departmentName))
            {
                quarterlySalesByDepartment[quarter][data.departmentName] += totalSales;
                quarterlyProfitByDepartment[quarter][data.departmentName] += profit;
            }
            else
            {
                quarterlySalesByDepartment[quarter][data.departmentName] = totalSales;
                quarterlyProfitByDepartment[quarter][data.departmentName] = profit;
            }

            var cumulativeDepartmentProfit = quarterlyProfitByDepartment[quarter][data.departmentName];
            var cumulativeDepartmentSales = quarterlySalesByDepartment[quarter][data.departmentName];
            quarterlyProfitPercentageByDepartment[quarter][data.departmentName] = (cumulativeDepartmentProfit / cumulativeDepartmentSales) * 100;

            if (quarterlySales.ContainsKey(quarter))
            {
                quarterlySales[quarter] += totalSales;
                quarterlyProfit[quarter] += profit;
            }
            else
            {
                quarterlySales[quarter] = totalSales;
                quarterlyProfit[quarter] = profit;
            }

            var cumulativeQuarterProfit = quarterlyProfit[quarter];
            var cumulativeQuarterSales = quarterlySales[quarter];
            quarterlyProfitPercentage[quarter] = (cumulativeQuarterProfit / cumulativeQuarterSales) * 100;

            if (!top3SalesOrdersByQuarter.ContainsKey(quarter))
            {
                top3SalesOrdersByQuarter[quarter] = new List<SalesData>();
            }

            top3SalesOrdersByQuarter[quarter].Add(data);

            var productSerialNumber = GenerateProductSerialNumber(data.productID);
            if (!productsSoldByQuarter.ContainsKey(quarter))
            {
                productsSoldByQuarter[quarter] = new List<string>();
            }
            productsSoldByQuarter[quarter].Add(productSerialNumber);
            }

            foreach (var quarter in top3SalesOrdersByQuarter.Keys)
            {
            top3SalesOrdersByQuarter[quarter] = top3SalesOrdersByQuarter[quarter]
                .OrderByDescending(order => (order.quantitySold * order.unitPrice) - (order.quantitySold * order.baseCost))
                .Take(3)
                .ToList();
            }

            DisplayQuarterlySalesReport(quarterlySales, quarterlyProfit, quarterlyProfitPercentage, quarterlySalesByDepartment, quarterlyProfitByDepartment, quarterlyProfitPercentageByDepartment, top3SalesOrdersByQuarter, productsSoldByQuarter, salesData);
        }

        private void DisplayQuarterlySalesReport(
            Dictionary<string, double> quarterlySales,
            Dictionary<string, double> quarterlyProfit,
            Dictionary<string, double> quarterlyProfitPercentage,
            Dictionary<string, Dictionary<string, double>> quarterlySalesByDepartment,
            Dictionary<string, Dictionary<string, double>> quarterlyProfitByDepartment,
            Dictionary<string, Dictionary<string, double>> quarterlyProfitPercentageByDepartment,
            Dictionary<string, List<SalesData>> top3SalesOrdersByQuarter,
            Dictionary<string, List<string>> productsSoldByQuarter,
            SalesData[] salesData)
        {
            Console.WriteLine("Quarterly Sales Report");
            Console.WriteLine("----------------------");

            var sortedQuarterlySales = quarterlySales.OrderBy(q => q.Key);

            foreach (var quarter in sortedQuarterlySales)
            {
            DisplayQuarterSummary(quarter, quarterlyProfit, quarterlyProfitPercentage);
            DisplayDepartmentSummary(quarter.Key, quarterlySalesByDepartment, quarterlyProfitByDepartment, quarterlyProfitPercentageByDepartment);
            DisplayTop3SalesOrders(quarter.Key, top3SalesOrdersByQuarter);
            DisplayTop3ProductsSold(quarter.Key, productsSoldByQuarter, salesData);
            }
        }

        private void DisplayQuarterSummary(KeyValuePair<string, double> quarter, Dictionary<string, double> quarterlyProfit, Dictionary<string, double> quarterlyProfitPercentage)
        {
            var formattedSalesAmount = quarter.Value.ToString("C");
            var formattedProfitAmount = quarterlyProfit[quarter.Key].ToString("C");
            var formattedProfitPercentage = quarterlyProfitPercentage[quarter.Key].ToString("F2");

            Console.WriteLine("{0}: Sales: {1}, Profit: {2}, Profit Percentage: {3}%", quarter.Key, formattedSalesAmount, formattedProfitAmount, formattedProfitPercentage);
        }

        private void DisplayDepartmentSummary(string quarterKey, Dictionary<string, Dictionary<string, double>> quarterlySalesByDepartment, Dictionary<string, Dictionary<string, double>> quarterlyProfitByDepartment, Dictionary<string, Dictionary<string, double>> quarterlyProfitPercentageByDepartment)
        {
            Console.WriteLine("By Department:");
            var sortedQuarterlySalesByDepartment = quarterlySalesByDepartment[quarterKey].OrderBy(d => d.Key);

            Console.WriteLine("┌───────────────────────┬───────────────────┬───────────────────┬───────────────────┐");
            Console.WriteLine("│      Department       │       Sales       │       Profit      │ Profit Percentage │");
            Console.WriteLine("├───────────────────────┼───────────────────┼───────────────────┼───────────────────┤");

            foreach (var department in sortedQuarterlySalesByDepartment)
            {
            var formattedDepartmentSalesAmount = department.Value.ToString("C");
            var formattedDepartmentProfitAmount = quarterlyProfitByDepartment[quarterKey][department.Key].ToString("C");
            var formattedDepartmentProfitPercentage = quarterlyProfitPercentageByDepartment[quarterKey][department.Key].ToString("F2");

            Console.WriteLine("│ {0,-22}│ {1,17} │ {2,17} │ {3,17} │", department.Key, formattedDepartmentSalesAmount, formattedDepartmentProfitAmount, formattedDepartmentProfitPercentage);
            }

            Console.WriteLine("└───────────────────────┴───────────────────┴───────────────────┴───────────────────┘");
            Console.WriteLine();
        }

        private void DisplayTop3SalesOrders(string quarterKey, Dictionary<string, List<SalesData>> top3SalesOrdersByQuarter)
        {
            Console.WriteLine("Top 3 Sales Orders:");
            var top3SalesOrders = top3SalesOrdersByQuarter[quarterKey];

            Console.WriteLine("┌───────────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┐");
            Console.WriteLine("│      Product ID       │   Quantity Sold   │    Unit Price     │   Total Sales     │      Profit       │ Profit Percentage │");
            Console.WriteLine("├───────────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┤");

            foreach (var salesOrder in top3SalesOrders)
            {
            var orderTotalSales = salesOrder.quantitySold * salesOrder.unitPrice;
            var orderProfit = orderTotalSales - (salesOrder.quantitySold * salesOrder.baseCost);
            var orderProfitPercentage = (orderProfit / orderTotalSales) * 100;

            Console.WriteLine("│ {0,-22}│ {1,17} │ {2,17} │ {3,17} │ {4,17} │ {5,17} │", salesOrder.productID, salesOrder.quantitySold, salesOrder.unitPrice.ToString("C"), orderTotalSales.ToString("C"), orderProfit.ToString("C"), orderProfitPercentage.ToString("F2"));
            }

            Console.WriteLine("└───────────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┘");
            Console.WriteLine();
        }

        private void DisplayTop3ProductsSold(string quarterKey, Dictionary<string, List<string>> productsSoldByQuarter, SalesData[] salesData)
        {
            Console.WriteLine("Top 3 Products Sold in {0}:", quarterKey);
            var sortedProductsSold = productsSoldByQuarter[quarterKey].OrderBy(p => p).ToList();

            Console.WriteLine("┌───────────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┬───────────────────┐");
            Console.WriteLine("│      Product ID       │   Quantity Sold   │    Unit Price     │   Total Sales     │      Profit       │ Profit Percentage │");
            Console.WriteLine("├───────────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┼───────────────────┤");

            var sortedProductsByProfit = sortedProductsSold
            .Select(serialNumber => salesData.FirstOrDefault(s => GenerateProductSerialNumber(s.productID) == serialNumber))
            .Where(salesOrder => salesOrder.productID != null)
            .OrderByDescending(salesOrder => (salesOrder.quantitySold * salesOrder.unitPrice) - (salesOrder.quantitySold * salesOrder.baseCost))
            .ToList();

            var top3Products = sortedProductsSold
            .Select(serialNumber => salesData.FirstOrDefault(s => GenerateProductSerialNumber(s.productID) == serialNumber))
            .Where(salesOrder => salesOrder.productID != null)
            .OrderByDescending(salesOrder => (salesOrder.quantitySold * salesOrder.unitPrice) - (salesOrder.quantitySold * salesOrder.baseCost))
            .Take(3);

            foreach (var salesOrder in top3Products)
            {
            var orderTotalSales = salesOrder.quantitySold * salesOrder.unitPrice;
            var orderProfit = orderTotalSales - (salesOrder.quantitySold * salesOrder.baseCost);
            var orderProfitPercentage = (orderProfit / orderTotalSales) * 100;

            Console.WriteLine("│ {0,-22}│ {1,17} │ {2,17} │ {3,17} │ {4,17} │ {5,17} │", salesOrder.productID, salesOrder.quantitySold, salesOrder.unitPrice.ToString("C"), orderTotalSales.ToString("C"), orderProfit.ToString("C"), orderProfitPercentage.ToString("F2"));
            }

            Console.WriteLine("└───────────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┴───────────────────┘");
            Console.WriteLine();
        }

        public string GetQuarter(int month)
        {
            if (month >= 1 && month <= 3)
            {
                return "Q1";
            }
            else if (month >= 4 && month <= 6)
            {
                return "Q2";
            }
            else if (month >= 7 && month <= 9)
            {
                return "Q3";
            }
            else
            {
                return "Q4";
            }
        }
    }
}
