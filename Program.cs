using System;
using System.Collections.Generic;
using System.IO;

namespace MtgExportsFixer
{
    class Program
    {
        class TappedOutCard
        {
            internal string Qty;
            internal string Name;
            internal string Printing;
            internal string Foil;
            internal string Alter;
            internal string Signed;
            internal string Condition;
            internal string Language;
        }

        static string columns;
        static List<string> output = new List<string>();
        static List<TappedOutCard> inputCards = new List<TappedOutCard>();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            try
            {
                using (StreamReader sw = new StreamReader(args[0]))
                {
                    columns = sw.ReadLine();
                    var deckStatsColumns = "amount,card_name,is_foil,is_pinned,set_id,set_code,collector_number,language,condition,comment,added";
                    output.Add(deckStatsColumns);
                    while (!sw.EndOfStream)
                    {
                        var line = sw.ReadLine();
                        int iteration = 0;
                        var outLine = "";
                        var inputCard = new TappedOutCard();

                        for (int i = 0; i <= columns.Split(',').Length; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    var portion = "";
                                    while (iteration < line.Length && line[iteration] != ',')
                                    {
                                        portion += line[iteration];
                                        iteration++;
                                    }
                                    iteration++;
                                    inputCard.Qty = portion.Trim('b').Trim('\'');
                                    //outLine += portion.Trim('b').Trim('\'') + ',';
                                    break;
                                case 1:
                                    if (line[iteration] == 'b')
                                    {
                                        portion = "";
                                        while (iteration < line.Length && line[iteration] != ',')
                                        {
                                            portion += line[iteration];
                                            iteration++;
                                        }
                                        iteration++;
                                        inputCard.Name = portion.Trim('b').Trim('\'');
                                        //outLine += portion.Trim('b').Trim('\'') + ',';
                                    }
                                    else if (line[iteration] == '"')
                                    {
                                        iteration ++;
                                        portion = "";
                                        bool bracketsOpen = false;
                                        while (iteration < line.Length && line[iteration] != '"')
                                        {
                                            portion += line[iteration];
                                            iteration++;
                                        }
                                        iteration++;
                                        while (iteration < line.Length && line[iteration] != ',' || bracketsOpen)
                                        {
                                            if (line[iteration] == '\"')
                                                bracketsOpen = !bracketsOpen;

                                            portion += line[iteration];
                                            iteration++;
                                        }
                                        iteration++;
                                        inputCard.Name = portion.Trim('"').Trim('b').Trim('"').Trim('\'');
                                        //outLine += '"'+portion.Trim('"').Trim('b').Trim('"').Trim('\'')+ '"' + ',';
                                    }
                                    else
                                    {
                                        portion = "";
                                        while (line[iteration] != ',')
                                        {
                                            portion += line[iteration];
                                            iteration++;
                                        }
                                        inputCard.Name = portion;
                                        iteration++;
                                    }
                                    break;
                                default:
                                    portion = "";
                                    while (iteration < line.Length && line[iteration] != ',')
                                    {
                                        portion += line[iteration];
                                        iteration++;
                                    }
                                    iteration++;
                                    string theVal = portion.Trim('b').Trim('\'');
                                    if (theVal == "-")
                                        theVal = "";

                                    switch (i)
                                    {
                                        case 2:
                                            inputCard.Printing = theVal;
                                            break;
                                        case 3:
                                            inputCard.Foil = theVal;
                                            break;
                                        case 4:
                                            inputCard.Alter = theVal;
                                            break;
                                        case 5:
                                            inputCard.Signed = theVal;
                                            break;
                                        case 6:
                                            inputCard.Condition = theVal;
                                            break;
                                        case 7:
                                            inputCard.Language = theVal;
                                            break;
                                    }
                                    //outLine += portion.Trim('b').Trim('\'') + ',';
                                    break;
                            }
                        }

                        //output.Add(outLine.Trim(','));
                        inputCards.Add(inputCard);
                    }
                    sw.Close();
                }

                foreach (var c in inputCards)
                {
                    if (!int.TryParse(c.Qty, out int qty))
                    {
                        Console.WriteLine("Parsing error.");
                        Console.ReadLine();
                        Environment.Exit(1);
                    }

                    int isFoil = c.Foil.Length > 0 ? 1 : 0;
                    if (c.Printing.ToUpper() == "EO2")
                        c.Printing = "E02";
                    else if (c.Printing.ToUpper() == "MYS1" || c.Printing.ToUpper() == "MYSTOR")
                        c.Printing = "MB1";
                    else if (c.Printing.ToUpper() == "PSG")
                        c.Printing = "PLGS";

                    if (c.Language.ToUpper() == "JA")
                        c.Language = "JP";

                    string deckStatsLine = $"{qty},\"{c.Name}\",{isFoil},\"\",\"\",\"{c.Printing}\",\"\",\"{c.Language.ToUpper()}\",\"{c.Condition}\",\"\",\"\"";
                    output.Add(deckStatsLine);
                }

                using (StreamWriter sz = new StreamWriter("output.csv"))
                {
                    foreach (string s in output)
                    {
                        sz.WriteLine(s);
                    }
                    sz.Flush();
                    sz.Close();
                }
            }
            catch (Exception) { }
        }
    }
}
