namespace CrossPlatformApp.CLI.Helpers;

public static class TableHelper
{
    public static void PrintTable<T>(IEnumerable<T> items, string[] headers, Func<T, string[]> getRowData)
    {
        if (!items.Any()) return;

        var rows = items.Select(getRowData).ToList();
        var columnWidths = new int[headers.Length];

        // Calculate column widths based on headers and data
        for (var i = 0; i < headers.Length; i++)
        {
            columnWidths[i] = headers[i].Length;
            foreach (var row in rows)
            {
                columnWidths[i] = Math.Max(columnWidths[i], row[i]?.Length ?? 0);
            }
        }

        // Print top border
        PrintRowBorder(columnWidths, '┌', '┐', '┬');

        // Print headers
        PrintRow(headers, columnWidths);

        // Print header separator
        PrintRowBorder(columnWidths, '├', '┤', '┼');

        // Print data rows
        foreach (var row in rows)
        {
            PrintRow(row, columnWidths);
        }

        // Print bottom border
        PrintRowBorder(columnWidths, '└', '┘', '┴');
    }

    private static void PrintRowBorder(int[] columnWidths, char left, char right, char separator)
    {
        Console.Write(left);
        for (var i = 0; i < columnWidths.Length; i++)
        {
            Console.Write(new string('─', columnWidths[i] + 2));
            if (i < columnWidths.Length - 1)
            {
                Console.Write(separator);
            }
        }
        Console.WriteLine(right);
    }

    private static void PrintRow(string[] values, int[] columnWidths)
    {
        Console.Write('│');
        for (var i = 0; i < values.Length; i++)
        {
            var value = values[i] ?? string.Empty;
            Console.Write($" {value.PadRight(columnWidths[i])} ");
            if (i < values.Length - 1)
            {
                Console.Write('│');
            }
        }
        Console.WriteLine('│');
    }
}
