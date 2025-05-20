using System;

class SimplexTiesinisProgramavimas
{
    const double Y1 = 8;
    const double Y2 = 10;  
    const double Y3 = 3;
    
    static double[][] Table = {
        new[] {  0.0,  2,  -3,   0,  -5,   0,   0,   0 },   
        new[] {  Y1 , -1,   1,  -1,  -1,   1,   0,   0 },   
        new[] {  Y2 ,  2,   4,   0,   0,   0,   1,   0 },   
        new[] {  Y3 ,  0,   0,   1,   1,   0,   0,   1 }    
    };
    static string[] names =
    { "VAL", "x1", "x2", "x3", "x4", "s1", "s2", "s3" }; // VAL reiskia value arba reiksme

   static bool IsSolution(double[][] Table)
    {
        for (int i = 1; i < Table[0].Length; i++)
        {
            if (Table[0][i] < 0)
            {
                return false;
            }
        }
        return true;
    }
    static int FindLeavingColumn(double[][] Table)
    {
        int enter = -1;                         
        double mostNeg = 0;               

        for (int j = 1; j < Table[0].Length; j++)
            if (Table[0][j] < mostNeg)            
            {
                mostNeg = Table[0][j];
                enter = j;
            }
        return enter;
    }
    
    static int FindLeavingRow(double[][] T, int leavingColum)
    {
        int leave = -1;
        double minRatio = double.MaxValue;

        for (int i = 1; i < T.Length; i++)      
            if (T[i][leavingColum] > 1e-12)
            {
                double r = T[i][0] / T[i][leavingColum];
                if (r < minRatio) { minRatio = r; leave = i; }
            }
        return leave;                        
    }
    static void ScaleRow(double[][] Table, int leavingRow, int leavingColumn)
    {
        double pivot = Table[leavingRow][leavingColumn];
        for (int i = 0; i < Table[0].Length; i++)
        {
            Table[leavingRow][i] /= pivot;
        }
    }
    static void AddRows(double[][] Table, int leavingColumn, int leavingRow)
    {
        for (int i = 0; i < Table.Length; i++)
        {
            if (i == leavingRow) continue;
            
            double factor = Table[i][leavingColumn];
            for(int j = 0; j < Table[0].Length; j++)
            {
                Table[i][j] -= factor * Table[leavingRow][j];
            }
        }
    }
    
    static double[][] Simplex(double[][] Table)
    {
        int functionCalls = 0;
        int iterationCount = 0;
        while (true)
        {
            if (IsSolution(Table)) break;
            
            int leavingColumn = FindLeavingColumn(Table);
            if (leavingColumn == -1) break;
            
            int leavingRow = FindLeavingRow(Table, leavingColumn);
            if (leavingRow == -1) break;
            
            ScaleRow(Table, leavingRow, leavingColumn);
            AddRows(Table, leavingColumn, leavingRow);
            
            functionCalls += 4;
            iterationCount++;
            PrintMatrix(names, Table);
        }
        Console.WriteLine($"{functionCalls} function calls and {iterationCount} iterations");
        return Table;
    }
    static void PrintMatrix(string[] names, double[][] M)
    {
        const int W = 9;             
        Console.WriteLine();          
        Console.Write("".PadLeft(W));
        for (int j = 0; j < names.Length; j++)
            Console.Write($"{names[j],W}");
        Console.WriteLine();
        
        for (int i = 0; i < M.Length; i++)
        {
            Console.Write(i == 0 ? "  z |".PadLeft(W) : $" AP{i} |".PadLeft(W)); //AP[i] reiskia apribojimas[i]
            for (int j = 0; j < M[0].Length; j++)
                Console.Write($"{M[i][j],W:G3}");
            Console.WriteLine();
        }
    }
    static void PrintSolution(double[][] T, string[] names)
    {
    int rows = T.Length;
    int cols = T[0].Length;
    const int W = 9;                    

    Console.WriteLine("\n Sprendimo matrica"); 

    PrintMatrix(names, Table);
    
    bool[] basic = new bool[cols];
    int[]  where = new int[cols];        

    for (int j = 1; j < cols; ++j)        
    {
        int rowOfOne = -1;
        bool isBasic = true;

        for (int i = 0; i < rows && isBasic; ++i)
        {
            double v = Math.Abs(T[i][j]);
            if (v < 1e-9) continue;       
            if (Math.Abs(v - 1) < 1e-9)
            {
                if (rowOfOne == -1)
                    rowOfOne = i;
                else
                    isBasic = false;     
            }
            else
                isBasic = false;          
        }

        if (isBasic && rowOfOne != -1)
        {
            basic[j] = true;
            where[j] = rowOfOne;
        }
    }

    Console.WriteLine("\nAtsakymas");
    for (int j = 1; j < cols; ++j)       
    {
        if (basic[j])
            Console.WriteLine($"{names[j],4} = {T[where[j]][0]:F3}");
        else
            Console.WriteLine($"{names[j],4} = 0");
    }

    double zMin = -T[0][0];
    Console.WriteLine($"\nTikslo funkcija = {zMin:F3}");
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Simplekso optimizavimas:");
        PrintMatrix(names, Table);
        Simplex(Table);
        PrintSolution(Table, names);
    }
}
