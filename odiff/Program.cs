using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SearchAThing;
using System.Linq;
using static System.Math;

namespace onlydiff
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmdline = new CmdlineParser("Shows only difference between files", (p) =>
            {
                var showStats = p.AddShortLong("s", "all-stats", "shows all stats informations");
                var showDiff = p.AddShortLong("d", "show-differences", "shows characters difference");

                var file1 = p.AddParameter("file1", "first file", mandatory: true);
                var file2 = p.AddParameter("file2", "second file", mandatory: true);

                p.OnCmdlineMatch = () =>
                {
                    var charDiffers1 = 0;
                    var charDiffers2 = 0;
                    var maxChangeWidth = 0;
                    var lineDiffers1 = 0;
                    var lineDiffers2 = 0;
                    var totalLines1 = 0;
                    var totalLines2 = 0;
                    var filesize1 = new FileInfo(file1).Length;
                    var filesize2 = new FileInfo(file2).Length;

                    #region detect maxChangeWidth
                    {
                        using (var sr1 = new StreamReader(file1))
                        {
                            using (var sr2 = new StreamReader(file2))
                            {
                                // detect max change width                            

                                while (!sr1.EndOfStream && !sr2.EndOfStream)
                                {
                                    var changed = 0;

                                    var line1 = sr1.ReadLine();
                                    var line2 = sr2.ReadLine();

                                    ++totalLines1;
                                    ++totalLines2;

                                    var len1 = line1.Length;
                                    var len2 = line2.Length;
                                    var lenmax = Max(len1, len2);

                                    int i1 = 0;
                                    int i2 = 0;

                                    while (true)
                                    {
                                        if (line1[i1] != line2[i2])
                                        {
                                            ++changed;

                                            if (i1 < len1 - 1 && line1[i1 + 1] == line2[i2])
                                            {
                                                ++charDiffers1;
                                                ++i1;
                                            }
                                            else if (i2 < len2 - 1 && line2[i2 + 1] == line1[i1])
                                            {
                                                ++i2;
                                                ++charDiffers2;
                                            }
                                            else
                                            {
                                                ++i1;
                                                ++i2;
                                                ++charDiffers1;
                                                ++charDiffers2;
                                            }
                                        }
                                        else
                                        {
                                            ++i1;
                                            ++i2;
                                        }
                                        if (i1 >= len1 && i2 >= len2) break;
                                        else if (i1 >= len1)
                                        {
                                            var r = len2 - i2 - 1;
                                            changed += r;
                                            charDiffers2 += r;
                                            break;
                                        }
                                        else if (i2 >= len2)
                                        {
                                            var r = len1 - i1 - 1;
                                            changed += r;
                                            charDiffers1 += r;
                                            break;
                                        }
                                    }

                                    if (changed > 0)
                                    {
                                        ++lineDiffers1;
                                        ++lineDiffers2;
                                    }

                                    maxChangeWidth = Max(maxChangeWidth, changed);
                                }

                                if (!sr1.EndOfStream)
                                {
                                    while (!sr1.EndOfStream)
                                    {
                                        var line1 = sr1.ReadLine();

                                        ++totalLines1;
                                        ++lineDiffers1;

                                        var len1 = line1.Length;

                                        maxChangeWidth = Max(maxChangeWidth, len1);
                                        charDiffers1 += len1;
                                    }
                                }

                                if (!sr2.EndOfStream)
                                {
                                    while (!sr2.EndOfStream)
                                    {
                                        var line2 = sr2.ReadLine();

                                        ++totalLines2;
                                        ++lineDiffers2;

                                        var len2 = line2.Length;

                                        maxChangeWidth = Max(maxChangeWidth, len2);
                                        charDiffers2 += len2;
                                    }
                                }

                                var tblOut = new List<List<string>>();

                                if (showStats)
                                {
                                    tblOut.Add(new List<string>() { "max changed chars per line", maxChangeWidth.ToString(), maxChangeWidth.ToString() });

                                    tblOut.Add(new List<string>() { "size", filesize1.ToString(), filesize2.ToString() });

                                    tblOut.Add(new List<string>() { "lines", totalLines1.ToString(), totalLines2.ToString() });

                                    tblOut.Add(new List<string>() { "line differs", lineDiffers1.ToString(), lineDiffers2.ToString() });

                                    tblOut.Add(new List<string>() { "line differs (%)",
                                        string.Format("{0:0.0}", (double)lineDiffers1 / totalLines1 * 100),
                                        string.Format("{0:0.0}", (double)lineDiffers2 / totalLines2 * 100)
                                    });

                                    tblOut.Add(new List<string>() { "char differs", charDiffers1.ToString(), charDiffers2.ToString() });

                                    tblOut.Add(new List<string>() { "char differs (%)",
                                        string.Format("{0:0.0}", (double)charDiffers1 / filesize1 * 100),
                                        string.Format("{0:0.0}", (double)charDiffers2 / filesize2 * 100)
                                    });
                                }

                                if (tblOut.Count > 0)
                                {
                                    System.Console.WriteLine(tblOut.TableFormat(
                                        new[]
                                        {
                                            "",
                                            "file1",
                                            "file2"
                                        },
                                        new[]
                                        {
                                            ColumnAlignment.left,
                                            ColumnAlignment.right,
                                            ColumnAlignment.right
                                        }));
                                }


                            }
                        }
                    }
                    #endregion

                    #region show char changes
                    if (showDiff || p.Items.Count(w => w.Type == CmdlineParseItemType.flag && w.Matches) == 0)
                    {
                        using (var sr1 = new StreamReader(file1))
                        {
                            using (var sr2 = new StreamReader(file2))
                            {
                                var W = Max(maxChangeWidth, "fileX".Length);
                                var firstLine = true;

                                while (!sr1.EndOfStream && !sr2.EndOfStream)
                                {
                                    var line1 = sr1.ReadLine();
                                    var line2 = sr2.ReadLine();

                                    var len1 = line1.Length;
                                    var len2 = line2.Length;
                                    var lenmax = Max(len1, len2);

                                    var sb1 = new StringBuilder();
                                    var sb2 = new StringBuilder();

                                    int i1 = 0;
                                    int i2 = 0;

                                    while (true)
                                    {
                                        if (line1[i1] != line2[i2])
                                        {
                                            if (i1 < len1 - 1 && line1[i1 + 1] == line2[i2])
                                            {
                                                sb1.Append(line1[i1]);
                                                ++i1;
                                            }
                                            else if (i2 < len2 - 1 && line2[i2 + 1] == line1[i1])
                                            {
                                                sb1.Append(line2[i2]);
                                                ++i2;
                                            }
                                            else
                                            {
                                                ++i1;
                                                ++i2;
                                            }
                                        }
                                        else
                                        {
                                            ++i1;
                                            ++i2;
                                        }
                                        if (i1 >= len1 && i2 >= len2) break;
                                        else if (i1 >= len1)
                                        {
                                            sb2.Append(line2.Substring(i2));
                                            break;
                                        }
                                        else if (i2 >= len2)
                                        {
                                            sb1.Append(line1.Substring(i1));
                                            break;
                                        }
                                    }

                                    /*

                                    for (int i = 0; i < lenmax; ++i)
                                    {
                                        if (i < len1 && i < len2)
                                        {
                                            if (line1[i] != line2[i])
                                            {
                                                sb1.Append(line1[i]);
                                                sb2.Append(line2[i]);
                                            }
                                        }
                                        else
                                        {
                                            if (i < len1)
                                                sb1.Append(line1[i]);
                                            else if (i < len2)
                                                sb2.Append(line2[i]);
                                        }
                                    }
*/
                                    if (firstLine)
                                    {
                                        System.Console.WriteLine($"{"file1".Align(W)} | {"file2".Align(W)}");
                                        firstLine = false;
                                    }
                                    System.Console.WriteLine($"{sb1.ToString().Align(W)} | {sb2.ToString().Align(W)}");
                                }

                                if (!sr1.EndOfStream)
                                {
                                    while (!sr1.EndOfStream)
                                    {
                                        var line1 = sr1.ReadLine();

                                        System.Console.WriteLine($"{line1.Align(W)} |");
                                    }
                                }

                                if (!sr2.EndOfStream)
                                {
                                    while (!sr2.EndOfStream)
                                    {
                                        var line2 = sr2.ReadLine();

                                        System.Console.WriteLine($"{(" ".Repeat(W))} | {line2}");
                                    }
                                }

                            }
                        }
                    }
                    #endregion

                };
            });

            cmdline.Run(args);
        }
    }
}
