using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TVDCEG.Ultis;

namespace TVDCEG
{
    public class DimBrick
    {
        private static DimBrick _instance;
        private DimBrick() { }
        public static DimBrick Instance => _instance ?? (_instance = new DimBrick());
        public List<FamilyInstance> GetBrick(Document doc)
        {
            AssemblyInstance assemblyInstance = doc.GetElement(doc.ActiveView.AssociatedAssemblyInstanceId) as AssemblyInstance;
            ICollection<ElementId> elementIds = assemblyInstance.GetMemberIds();
            List<FamilyInstance> listbrick = (from x in elementIds where doc.GetElement(x).Name.Contains("BRICK") select doc.GetElement(x)).Cast<FamilyInstance>().ToList();
            return listbrick;
        }
        public Dictionary<string, List<BrickInformation>> SortBrickVertical(Document doc, Selection sel)
        {
            Dictionary<string, List<BrickInformation>> dic = new Dictionary<string, List<BrickInformation>>();
            List<FamilyInstance> listbrick = GetBrick(doc);
            //BrickInformation brick1 = new BrickInformation((FamilyInstance)doc.GetElement((sel.PickObject(ObjectType.Element, new FilterBrick(), "Select Brick"))));
            //BrickInformation brick2 = new BrickInformation((FamilyInstance)doc.GetElement((sel.PickObject(ObjectType.Element, new FilterBrick(), "Select Brick"))));
            //XYZ dicrect = brick1.Location - brick2.Location;
            XYZ dicrect = doc.ActiveView.UpDirection;
            if (Math.Round(dicrect.X, 1) != 0 && Math.Round(dicrect.Y, 1) == 0 && Math.Round(dicrect.Z, 1) == 0)
            {
                foreach (var i in listbrick)
                {
                    if (dic.ContainsKey(Math.Round(i.GetTransform().Origin.Y, 1).ToString() + Math.Round(i.GetTransform().Origin.Z, 1).ToString()))
                    {
                        dic[Math.Round(i.GetTransform().Origin.Y, 1).ToString() + Math.Round(i.GetTransform().Origin.Z, 1).ToString()].Add(new BrickInformation(i));
                    }
                    else
                    {
                        dic.Add(Math.Round(i.GetTransform().Origin.Y, 1).ToString() + Math.Round(i.GetTransform().Origin.Z, 1).ToString(), new List<BrickInformation> { new BrickInformation(i) });
                    }
                }
            }
            if (Math.Round(dicrect.X, 1) == 0 && Math.Round(dicrect.Y, 1) == 0 && Math.Round(dicrect.Z, 1) != 0)
            {
                foreach (var i in listbrick)
                {
                    if (dic.ContainsKey(Math.Round(i.GetTransform().Origin.X, 1).ToString() + Math.Round(i.GetTransform().Origin.Y, 1).ToString()))
                    {
                        dic[Math.Round(i.GetTransform().Origin.X, 1).ToString() + Math.Round(i.GetTransform().Origin.Y, 1).ToString()].Add(new BrickInformation(i));
                    }
                    else
                    {
                        dic.Add(Math.Round(i.GetTransform().Origin.X, 1).ToString() + Math.Round(i.GetTransform().Origin.Y, 1).ToString(), new List<BrickInformation> { new BrickInformation(i) });
                    }
                }
            }
            if (Math.Round(dicrect.X, 1) == 0 && Math.Round(dicrect.Y, 1) != 0 && Math.Round(dicrect.Z, 1) == 0)
            {
                foreach (var i in listbrick)
                {
                    if (dic.ContainsKey(Math.Round(i.GetTransform().Origin.X, 1).ToString() + Math.Round(i.GetTransform().Origin.Z, 1).ToString()))
                    {
                        dic[Math.Round(i.GetTransform().Origin.X, 1).ToString() + Math.Round(i.GetTransform().Origin.Z, 1).ToString()].Add(new BrickInformation(i));
                    }
                    else
                    {
                        dic.Add(Math.Round(i.GetTransform().Origin.X, 1).ToString() + Math.Round(i.GetTransform().Origin.Z, 1).ToString(), new List<BrickInformation> { new BrickInformation(i) });
                    }
                }
            }
            SortlistBrick(dic, dicrect);
            return dic;
        }
        public Dictionary<string, List<BrickInformation>> SortBrickHolizontal(Document doc, Selection sel)
        {
            Dictionary<string, List<BrickInformation>> dic = new Dictionary<string, List<BrickInformation>>();
            List<FamilyInstance> listbrick = GetBrick(doc);
            XYZ dicrect = doc.ActiveView.RightDirection;
            if (Math.Round(dicrect.X, 1) != 0 && Math.Round(dicrect.Y, 1) == 0 && Math.Round(dicrect.Z, 1) == 0)
            {
                foreach (var i in listbrick)
                {
                    if (dic.ContainsKey(Math.Round(i.GetTransform().Origin.Y, 1).ToString() + Math.Round(i.GetTransform().Origin.Z, 1).ToString()))
                    {
                        dic[Math.Round(i.GetTransform().Origin.Y, 1).ToString() + Math.Round(i.GetTransform().Origin.Z, 1).ToString()].Add(new BrickInformation(i));
                    }
                    else
                    {
                        dic.Add(Math.Round(i.GetTransform().Origin.Y, 1).ToString() + Math.Round(i.GetTransform().Origin.Z, 1).ToString(), new List<BrickInformation> { new BrickInformation(i) });
                    }
                }
            }
            if (Math.Round(dicrect.X, 1) == 0 && Math.Round(dicrect.Y, 1) == 0 && Math.Round(dicrect.Z, 1) != 0)
            {
                foreach (var i in listbrick)
                {
                    if (dic.ContainsKey(Math.Round(i.GetTransform().Origin.X, 1).ToString() + Math.Round(i.GetTransform().Origin.Y, 1).ToString()))
                    {
                        dic[Math.Round(i.GetTransform().Origin.X, 1).ToString() + Math.Round(i.GetTransform().Origin.Y, 1).ToString()].Add(new BrickInformation(i));
                    }
                    else
                    {
                        dic.Add(Math.Round(i.GetTransform().Origin.X, 1).ToString() + Math.Round(i.GetTransform().Origin.Y, 1).ToString(), new List<BrickInformation> { new BrickInformation(i) });
                    }
                }
            }
            if (Math.Round(dicrect.X, 1) == 0 && Math.Round(dicrect.Y, 1) != 0 && Math.Round(dicrect.Z, 1) == 0)
            {
                foreach (var i in listbrick)
                {
                    if (dic.ContainsKey(Math.Round(i.GetTransform().Origin.X, 1).ToString() + Math.Round(i.GetTransform().Origin.Z, 1).ToString()))
                    {
                        dic[Math.Round(i.GetTransform().Origin.X, 1).ToString() + Math.Round(i.GetTransform().Origin.Z, 1).ToString()].Add(new BrickInformation(i));
                    }
                    else
                    {
                        dic.Add(Math.Round(i.GetTransform().Origin.X, 1).ToString() + Math.Round(i.GetTransform().Origin.Z, 1).ToString(), new List<BrickInformation> { new BrickInformation(i) });
                    }
                }
            }
            SortlistBrick(dic, dicrect);
            return dic;
        }
        public Dictionary<string, List<BrickInformation>> SortBrickHolizontal2(Document doc, Selection sel)
        {
            Dictionary<string, List<BrickInformation>> dic = new Dictionary<string, List<BrickInformation>>();
            List<FamilyInstance> listbrick = GetBrick(doc);
            XYZ dicrect = doc.ActiveView.RightDirection;
            XYZ updirection = doc.ActiveView.UpDirection;
            if (Math.Round(dicrect.X, 1) != 0 && Math.Round(dicrect.Y, 1) == 0 && Math.Round(dicrect.Z, 1) == 0)
            {
                if (Math.Round(updirection.X, 1) == 0 && Math.Round(updirection.Y, 1) == 0 && Math.Round(updirection.Z, 1) != 0)
                {
                    foreach (var i in listbrick)
                    {
                        if (dic.ContainsKey(Math.Round(i.GetTransform().Origin.Z, 1).ToString()))
                        {
                            dic[Math.Round(i.GetTransform().Origin.Z, 1).ToString()].Add(new BrickInformation(i));
                        }
                        else
                        {
                            dic.Add(Math.Round(i.GetTransform().Origin.Z, 1).ToString(), new List<BrickInformation> { new BrickInformation(i) });
                        }
                    }
                }
                if (Math.Round(updirection.X, 1) == 0 && Math.Round(updirection.Y, 1) != 0 && Math.Round(updirection.Z, 1) == 0)
                {
                    foreach (var i in listbrick)
                    {
                        if (dic.ContainsKey(Math.Round(i.GetTransform().Origin.Y, 1).ToString()))
                        {
                            dic[Math.Round(i.GetTransform().Origin.Y, 1).ToString()].Add(new BrickInformation(i));
                        }
                        else
                        {
                            dic.Add(Math.Round(i.GetTransform().Origin.Y, 1).ToString(), new List<BrickInformation> { new BrickInformation(i) });
                        }
                    }
                }
            }
            if (Math.Round(dicrect.X, 1) == 0 && Math.Round(dicrect.Y, 1) == 0 && Math.Round(dicrect.Z, 1) != 0)
            {
                if (Math.Round(updirection.X, 1) != 0 && Math.Round(updirection.Y, 1) == 0 && Math.Round(updirection.Z, 1) == 0)
                {
                    foreach (var i in listbrick)
                    {
                        if (dic.ContainsKey(Math.Round(i.GetTransform().Origin.X, 1).ToString()))
                        {
                            dic[Math.Round(i.GetTransform().Origin.X, 1).ToString()].Add(new BrickInformation(i));
                        }
                        else
                        {
                            dic.Add(Math.Round(i.GetTransform().Origin.X, 1).ToString(), new List<BrickInformation> { new BrickInformation(i) });
                        }
                    }
                }
                if (Math.Round(updirection.X, 1) == 0 && Math.Round(updirection.Y, 1) != 0 && Math.Round(updirection.Z, 1) == 0)
                {
                    foreach (var i in listbrick)
                    {
                        if (dic.ContainsKey(Math.Round(i.GetTransform().Origin.Y, 1).ToString()))
                        {
                            dic[Math.Round(i.GetTransform().Origin.Y, 1).ToString()].Add(new BrickInformation(i));
                        }
                        else
                        {
                            dic.Add(Math.Round(i.GetTransform().Origin.Y, 1).ToString(), new List<BrickInformation> { new BrickInformation(i) });
                        }
                    }
                }
            }
            if (Math.Round(dicrect.X, 1) == 0 && Math.Round(dicrect.Y, 1) != 0 && Math.Round(dicrect.Z, 1) == 0)
            {
                if (Math.Round(updirection.X, 1) != 0 && Math.Round(updirection.Y, 1) == 0 && Math.Round(updirection.Z, 1) == 0)
                {
                    foreach (var i in listbrick)
                    {
                        if (dic.ContainsKey(Math.Round(i.GetTransform().Origin.X, 1).ToString()))
                        {
                            dic[Math.Round(i.GetTransform().Origin.X, 1).ToString()].Add(new BrickInformation(i));
                        }
                        else
                        {
                            dic.Add(Math.Round(i.GetTransform().Origin.X, 1).ToString(), new List<BrickInformation> { new BrickInformation(i) });
                        }
                    }
                }
                if (Math.Round(updirection.X, 1) == 0 && Math.Round(updirection.Y, 1) == 0 && Math.Round(updirection.Z, 1) != 0)
                {
                    foreach (var i in listbrick)
                    {
                        if (dic.ContainsKey(Math.Round(i.GetTransform().Origin.Z, 1).ToString()))
                        {
                            dic[Math.Round(i.GetTransform().Origin.Z, 1).ToString()].Add(new BrickInformation(i));
                        }
                        else
                        {
                            dic.Add(Math.Round(i.GetTransform().Origin.Z, 1).ToString(), new List<BrickInformation> { new BrickInformation(i) });
                        }
                    }
                }
            }
            SortlistBrick(dic, dicrect);
            return dic;
        }
        public void SortlistBrick(Dictionary<string, List<BrickInformation>> dic, XYZ direction)
        {
            foreach (var text in dic.Keys.ToList())
            {
                if (Math.Round(direction.X, 0) != 0 && Math.Round(direction.Y, 0) == 0 && Math.Round(direction.Z, 0) == 0)
                {
                    for (int i = 0; i < dic[text].Count; i++)
                    {
                        for (int j = 0; j < dic[text].Count; j++)
                        {
                            if (dic[text][i].Location.X < dic[text][j].Location.X)
                            {
                                var temp = dic[text][i];
                                dic[text][i] = dic[text][j];
                                dic[text][j] = temp;
                            }
                        }
                    }
                }
                if (Math.Round(direction.X, 0) == 0 && Math.Round(direction.Y, 0) != 0 && Math.Round(direction.Z, 0) == 0)
                {
                    for (int i = 0; i < dic[text].Count; i++)
                    {
                        for (int j = 0; j < dic[text].Count; j++)
                        {
                            if (dic[text][i].Location.Y < dic[text][j].Location.Y)
                            {
                                var temp = dic[text][i];
                                dic[text][i] = dic[text][j];
                                dic[text][j] = temp;
                            }
                        }
                    }
                }
                if (Math.Round(direction.X, 0) == 0 && Math.Round(direction.Y, 0) == 0 && Math.Round(direction.Z, 0) != 0)
                {
                    for (int i = 0; i < dic[text].Count; i++)
                    {
                        for (int j = 0; j < dic[text].Count; j++)
                        {
                            if (dic[text][i].Location.Z < dic[text][j].Location.Z)
                            {
                                var temp = dic[text][i];
                                dic[text][i] = dic[text][j];
                                dic[text][j] = temp;
                            }
                        }
                    }
                }
            }
        }
        Dictionary<string, List<BrickInformation>> RemoveRowBrick(Document doc, Selection sel, XYZ point)
        {
            Dictionary<string, List<BrickInformation>> dic = SortBrickVertical(doc, sel);
            Dictionary<string, List<BrickInformation>> dic2 = new Dictionary<string, List<BrickInformation>>();
            Dictionary<string, List<BrickInformation>> dic3 = new Dictionary<string, List<BrickInformation>>();
            var listkey = dic.Keys.ToList();
            foreach (var text in listkey)
            {
                var value = dic[text];
                string stringkey = value[0].ControlMark;
                for (int i = 1; i < value.Count; i++)
                {
                    stringkey = stringkey + ";" + value[i].ControlMark;
                }
                var keys = stringkey.Split(';').ToList();
                RemoveContankey(keys);
                var bk = Unionstring(keys) + ";" + text + ";" + value.Count.ToString();
                if (dic2.ContainsKey(bk))
                {
                    dic2[bk].Add((from x in value select x).First());
                }
                else
                {
                    dic2.Add(bk, value);
                }
            }
            foreach (var item in dic2.Keys.ToList())
            {
                if (dic2[item].Count > 1)
                {
                    var num1 = dic2[item].First();
                    var num2 = dic2[item].Last();
                    Line line = Line.CreateBound(num1.Location, num2.Location);
                    double kc = line.Distance(point);

                    dic3.Add(item + "-" + kc.ToString(), dic2[item]);
                }
            }
            return Filterdic(dic3);
        }
        Dictionary<string, List<BrickInformation>> RemoveRowBrick2(Document doc, Selection sel, XYZ point)
        {
            Dictionary<string, List<BrickInformation>> dic = SortBrickHolizontal(doc, sel);
            Dictionary<string, List<BrickInformation>> dic2 = new Dictionary<string, List<BrickInformation>>();
            Dictionary<string, List<BrickInformation>> dic3 = new Dictionary<string, List<BrickInformation>>();
            var listkey = dic.Keys.ToList();
            foreach (var text in listkey)
            {
                var value = dic[text];
                string stringkey = value[0].ControlMark;
                for (int i = 1; i < value.Count; i++)
                {
                    stringkey = stringkey + ";" + value[i].ControlMark;
                }
                var keys = stringkey.Split(';').ToList();
                RemoveContankey(keys);
                var bk = Unionstring(keys) + ";" + text + ";" + value.Count.ToString();
                if (dic2.ContainsKey(bk))
                {
                    dic2[bk].Add((from x in value select x).First());
                }
                else
                {
                    dic2.Add(bk, value);
                }
            }
            foreach (var item in dic2.Keys.ToList())
            {
                if (dic2[item].Count > 1)
                {
                    var num1 = dic2[item].First();
                    var num2 = dic2[item].Last();
                    Line line = Line.CreateBound(num1.Location, num2.Location);
                    double kc = line.Distance(point);

                    dic3.Add(item + "-" + kc.ToString(), dic2[item]);
                }
            }
            return Filterdic(dic3);
        }
        public void Createtextnote(Document doc, Dictionary<string, List<string>> dic)
        {
            List<string> elementIds = new List<string>();
            foreach (var item in dic.Keys)
            {
                var list = dic[item];
                var typetextnot = (from TextNoteType x in new FilteredElementCollector(doc).OfClass(typeof(TextNoteType)) where x.Name == "text brick" select x).First();
                ElementId defaultTextTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
                double noteWidth = 0.009;
                double minWidth = TextNote.GetMinimumAllowedWidth(doc, defaultTextTypeId);
                double maxWidth = TextNote.GetMaximumAllowedWidth(doc, defaultTextTypeId);
                TextNoteOptions opts = new TextNoteOptions(defaultTextTypeId);
                opts.TypeId = typetextnot.Id;
                opts.HorizontalAlignment = HorizontalTextAlignment.Left;
                using (Transaction tran = new Transaction(doc, "Create text note"))
                {

                    tran.Start();
                    for (int i = 0; i < list.Count; i++)
                    {
                        var id = Convert.ToInt32(list[i]);
                        ElementId hj = new ElementId(id);
                        FamilyInstance familyInstance = doc.GetElement(hj) as FamilyInstance;
                        TextNote textNote = TextNote.Create(doc, doc.ActiveView.Id, (familyInstance.GetTransform().Origin), noteWidth, i.ToString(), opts);
                        elementIds.Add(textNote.Id.IntegerValue.ToString());
                    }
                    tran.Commit();
                }
            }
            string folderPath = SettingExtension.GetSettingPath() + "\\TVD.11.SettingBrick";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            string contents = string.Empty;
            contents = JsonConvert.SerializeObject(elementIds, Formatting.Indented);
            string filePath = folderPath + "\\" + "Deletetextnote.json";
            File.WriteAllText(filePath, contents);
        }
        Dictionary<string, List<BrickInformation>> Filterdic(Dictionary<string, List<BrickInformation>> dic)
        {
            Dictionary<string, List<BrickInformation>> dic2 = new Dictionary<string, List<BrickInformation>>();
            List<string> Key = new List<string>();
            var listkeys = dic.Keys.ToList();
            listkeys.Sort();
            for (int i = 1; i < listkeys.Count; i++)
            {
                for (int j = i - 1; j < i; j++)
                {
                    string str1 = Splitkey(listkeys[i]);
                    string str2 = Splitkey(listkeys[j]);
                    if (str1 == str2)
                    {
                        listkeys.Remove(listkeys[j]);
                        i--;
                    }
                }
            }
            foreach (var item in listkeys)
            {
                try
                {
                    var newlistkey = dic.Keys.ToList();
                    var listcontain = (from x in newlistkey where Splitkey(item) == Splitkey(x) select x).ToList();
                    var bn = listcontain.First();
                    double giatri = Convert.ToDouble(bn.Split(';').ToList().Last().Split('-').Last());
                    foreach (var j in listcontain)
                    {
                        var vs = j.Split(';').ToList();
                        double _giatri = Convert.ToDouble(vs.Last().Split(';').Last().Split('-').Last());
                        if (_giatri < giatri)
                        {
                            bn = j;
                        }
                    }
                    dic2.Add(bn, dic[bn]);
                }
                catch
                {
                    //var newlistkey = dic.Keys.ToList();
                    //var listcontain = (from x in newlistkey where Splitkey(item) == Splitkey(x) select x).ToList();
                    //var bn = listcontain.First();
                    //double giatri = Convert.ToDouble(bn.Split(';').ToList().Last().Split('-').Last());
                    //foreach (var j in listcontain)
                    //{
                    //    var vs = j.Split(';').ToList();
                    //    double _giatri = Convert.ToDouble(vs.Last().Split(';').Last().Split('-').Last());
                    //    if (_giatri < giatri)
                    //    {
                    //        bn = j;
                    //    }
                    //}
                    //dic2.Add(bn, dic[bn]);
                    return Sortdic(dic2);
                }

            }
            return Sortdic(dic2);
        }
        public Dictionary<string, List<BrickInformation>> Sortdic(Dictionary<string, List<BrickInformation>> dic)
        {
            Dictionary<string, List<BrickInformation>> dic3 = new Dictionary<string, List<BrickInformation>>();
            var Ikey = dic.Keys.ToList();
            for (int i = 0; i < Ikey.Count; i++)
            {
                for (int j = 0; j < Ikey.Count; j++)
                {
                    if (Convert.ToDouble(Ikey[i].Split(';').ToList().Last().Split('-').Last()) < Convert.ToDouble(Ikey[j].Split(';').ToList().Last().Split('-').Last()))
                    {
                        var temp = Ikey[i];
                        Ikey[i] = Ikey[j];
                        Ikey[j] = temp;
                    }
                }
            }
            Ikey.ForEach(x => dic3.Add(x, dic[x]));
            return dic3;
        }
        string Splitkey(string key)
        {
            var list = key.Split(';').ToList();
            string newstring = list.First() + ";" + list.Last().Split('-').First();
            return newstring;
        }
        List<string> RemoveContankey(List<string> list)
        {
            for (int i = 1; i < list.Count; i++)
            {
                for (int j = i - 1; j < i; j++)
                {
                    string str1 = list[i];
                    string str2 = list[j];
                    if (str1 == str2)
                    {
                        list.RemoveAt(j);
                        i--;
                    }
                }
            }
            return list;
        }
        string Unionstring(List<string> list)
        {
            string cl = list[0];
            for (int i = 1; i < list.Count; i++)
            {
                cl = cl + "-" + list[i];
            }
            return cl;
        }
        public void CreateDimBrickVertical(Document doc, Selection sel, DimensionType dimensionType)
        {
            XYZ p1 = sel.PickPoint();
            Dictionary<string, List<BrickInformation>> dic = RemoveRowBrick(doc, sel, p1);
            //try
            //{
            //    for (int i = 0, j = 0; i < dic.Keys.Count; i++, j += 2)
            //    {
            //        XYZ direction = (dic[dic.Keys.ToList()[i]].First().Location - dic[dic.Keys.ToList()[i]].Last().Location);
            //        ReferenceArray referenceArray = new ReferenceArray();
            //        PlanarFace rightface1;
            //        PlanarFace leftface1;
            //        GetReferenceBrick(direction, dic[dic.Keys.ToList()[i]].First().brickinstance, out rightface1, out leftface1);
            //        referenceArray.Append(rightface1.Reference);
            //        referenceArray.Append(leftface1.Reference);
            //        PlanarFace rightface2;
            //        PlanarFace leftface2;
            //        GetReferenceBrick(direction, dic[dic.Keys.ToList()[i]].Last().brickinstance, out rightface2, out leftface2);
            //        referenceArray.Append(rightface2.Reference);
            //        referenceArray.Append(leftface2.Reference);
            //        XYZ p2 = p1 + j * doc.ActiveView.RightDirection;
            //        XYZ endp = p2 + direction;
            //        Line line = Line.CreateBound(p2, endp);
            //        using (Transaction tran = new Transaction(doc, "Create dim brick"))
            //        {
            //            tran.Start();
            //            Dimension dimension = doc.Create.NewDimension(doc.ActiveView, line, referenceArray, dimensionType);
            //            AddAnotationVertical(dimension, dic.Keys.ToList()[i], dic);
            //            tran.Commit();
            //        }
            //        Dictionary<string, List<string>> dic3 = new Dictionary<string, List<string>>();
            //        foreach (var item in dic.Keys)
            //        {
            //            var gk = new List<string>();
            //            dic[item].ToList().ForEach(x => gk.Add(x.ID));
            //            dic3.Add(item, gk);
            //        }
            //        Savebrickinformation(dic3);
            //    }
            //}
            //catch
            //{

            //}
            for (int i = 0, j = 0; i < dic.Keys.Count; i++, j += 2)
            {
                XYZ direction = (dic[dic.Keys.ToList()[i]].First().Location - dic[dic.Keys.ToList()[i]].Last().Location);
                ReferenceArray referenceArray = new ReferenceArray();
                PlanarFace rightface1;
                PlanarFace leftface1;
                GetReferenceBrick(direction, dic[dic.Keys.ToList()[i]].First().brickinstance, out rightface1, out leftface1);
                referenceArray.Append(rightface1.Reference);
                referenceArray.Append(leftface1.Reference);
                PlanarFace rightface2;
                PlanarFace leftface2;
                GetReferenceBrick(direction, dic[dic.Keys.ToList()[i]].Last().brickinstance, out rightface2, out leftface2);
                referenceArray.Append(rightface2.Reference);
                referenceArray.Append(leftface2.Reference);
                XYZ p2 = p1 + j * doc.ActiveView.RightDirection;
                XYZ endp = p2 + direction;
                Line line = Line.CreateBound(p2, endp);
                using (Transaction tran = new Transaction(doc, "Create dim brick"))
                {
                    tran.Start();
                    Dimension dimension = doc.Create.NewDimension(doc.ActiveView, line, referenceArray, dimensionType);
                    AddAnotationVertical(dimension, dic.Keys.ToList()[i], dic);
                    tran.Commit();
                }
                Dictionary<string, List<string>> dic3 = new Dictionary<string, List<string>>();
                foreach (var item in dic.Keys)
                {
                    var gk = new List<string>();
                    dic[item].ToList().ForEach(x => gk.Add(x.ID));
                    dic3.Add(item, gk);
                }
                Savebrickinformation(dic3);
            }
        }
        public void CreateDimBrickVertical2(Document doc, Selection sel, DimensionType dimensionType)
        {
            XYZ p1 = sel.PickPoint();
            Dictionary<string, List<BrickInformation>> dic = RemoveRowBrick2(doc, sel, p1);
            try
            {
                for (int i = 0, j = 0; i < dic.Keys.Count; i++, j += 2)
                {
                    XYZ direction = (dic[dic.Keys.ToList()[i]].First().Location - dic[dic.Keys.ToList()[i]].Last().Location);
                    ReferenceArray referenceArray = new ReferenceArray();
                    PlanarFace rightface1;
                    PlanarFace leftface1;
                    GetReferenceBrick(direction, dic[dic.Keys.ToList()[i]].First().brickinstance, out rightface1, out leftface1);
                    referenceArray.Append(rightface1.Reference);
                    referenceArray.Append(leftface1.Reference);
                    PlanarFace rightface2;
                    PlanarFace leftface2;
                    GetReferenceBrick(direction, dic[dic.Keys.ToList()[i]].Last().brickinstance, out rightface2, out leftface2);
                    referenceArray.Append(rightface2.Reference);
                    referenceArray.Append(leftface2.Reference);
                    XYZ p2 = p1 - j * doc.ActiveView.UpDirection;
                    XYZ endp = p2 + direction;
                    Line line = Line.CreateBound(p2, endp);
                    using (Transaction tran = new Transaction(doc, "Create dim brick"))
                    {
                        tran.Start();
                        Dimension dimension = doc.Create.NewDimension(doc.ActiveView, line, referenceArray, dimensionType);
                        AddAnotationVertical2(dimension, dic.Keys.ToList()[i], dic);
                        tran.Commit();
                    }
                    Dictionary<string, List<string>> dic3 = new Dictionary<string, List<string>>();
                    foreach (var item in dic.Keys)
                    {
                        var gk = new List<string>();
                        dic[item].ToList().ForEach(x => gk.Add(x.ID));
                        dic3.Add(item, gk);
                    }
                    Savebrickinformation(dic3);
                }
            }
            catch
            {

            }
        }
        void Savebrickinformation(Dictionary<string, List<string>> dic)
        {
            string folderPath = SettingExtension.GetSettingPath() + "\\TVD.11.SettingBrick";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            string contents = string.Empty;
            contents = JsonConvert.SerializeObject(dic, Formatting.Indented);
            string filePath = folderPath + "\\" + "BrickSave.json";
            File.WriteAllText(filePath, contents);
        }
        public void Showbrickdim(Document doc)
        {
            try
            {
                var filepath = SettingExtension.GetSettingPath() + "\\TVD.11.SettingBrick" + "\\" + "BrickSave.json";
                if (File.Exists(filepath))
                {
                    string value = File.ReadAllText(filepath);
                    var newdata = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(value);
                    Createtextnote(doc, newdata);
                }
            }
            catch
            {

            }
        }
        public void Deletetexnote(Document doc)
        {
            try
            {
                using (Transaction tr = new Transaction(doc, "Delete text note"))
                {
                    tr.Start();
                    var filepath = SettingExtension.GetSettingPath() + "\\TVD.11.SettingBrick" + "\\" + "Deletetextnote.json";
                    if (File.Exists(filepath))
                    {
                        string value = File.ReadAllText(filepath);
                        var newdata = JsonConvert.DeserializeObject<List<string>>(value);
                        ICollection<ElementId> list = new List<ElementId>();
                        foreach (var item in newdata)
                        {
                            var id = Convert.ToInt32(item);
                            ElementId hj = new ElementId(id);
                            list.Add(hj);
                        }
                        doc.Delete(list);
                    }
                    tr.Commit();
                }
            }
            catch
            {

            }
        }
        #region cach1
        //public Dictionary<string, List<BrickInformation>> CaculateRowbrick(Document doc, Dictionary<string, List<BrickInformation>> dic, Selection sel, out BrickInformation brick1, out BrickInformation brick2, out string k1, out string k2)
        //{
        //    Dictionary<string, List<BrickInformation>> dic1 = new Dictionary<string, List<BrickInformation>>();
        //    brick1 = new BrickInformation((FamilyInstance)doc.GetElement((sel.PickObject(ObjectType.Element, new FilterBrick(), "Select Brick"))));
        //    brick2 = new BrickInformation((FamilyInstance)doc.GetElement((sel.PickObject(ObjectType.Element, new FilterBrick(), "Select Brick"))));
        //    string key1 = null;
        //    string key2 = null;
        //    var listkey = dic.Keys.ToList();
        //    foreach (var text in listkey)
        //    {
        //        foreach (var item in dic[text])
        //        {
        //            if (brick1.ID == item.ID)
        //            {
        //                key1 = text;
        //            }
        //            if (brick2.ID == item.ID)
        //            {
        //                key2 = text;
        //            }
        //        }
        //    }
        //    k1 = key1;
        //    k2 = key2;
        //    var firtvalue = listkey.IndexOf(key1);
        //    var lastvalue = listkey.LastIndexOf(key2);
        //    List<string> list = new List<string>();
        //    if (firtvalue < lastvalue)
        //    {
        //        for (int i = firtvalue; i < lastvalue; i++)
        //        {
        //            list.Add(listkey[i]);
        //        }
        //        list.Add(listkey[lastvalue]);
        //    }
        //    else
        //    {
        //        for (int i = lastvalue; i < firtvalue; i++)
        //        {
        //            list.Add(listkey[i]);
        //        }
        //        list.Add(listkey[firtvalue]);
        //    }
        //    list.ForEach(x => dic1.Add(x, dic[x]));
        //    return dic1;
        //}
        //public void CreateDimBrickHolizontal(Document doc, Selection sel, int space,DimensionType dimensionType1,DimensionType dimensionType2)
        //{
        //    BrickInformation brick1;
        //    BrickInformation brick2;
        //    string key1;
        //    string key2;
        //    Dictionary<string, List<BrickInformation>> dic1 = Getrowbrick(doc, sel);
        //    Dictionary<string, List<BrickInformation>> dic = CaculateRowbrick(doc, dic1, sel, out brick1, out brick2, out key1, out key2);
        //    try
        //    {
        //        Reference rf = sel.PickObject(ObjectType.Edge);
        //        //XYZ point1 = sel.PickPoint();
        //        //XYZ point2 = sel.PickPoint();
        //        XYZ direction = (doc.ActiveView.RightDirection);
        //        ReferenceArray referenceArray1 = new ReferenceArray();
        //        ReferenceArray referenceArray = new ReferenceArray();
        //        referenceArray.Append(rf);
        //        PlanarFace rightface1;
        //        GetReferenceBrickholizontal(direction, brick1.brickinstance, out rightface1);
        //        referenceArray.Append(rightface1.Reference);
        //        referenceArray1.Append(rightface1.Reference);
        //        PlanarFace rightface2;
        //        GetReferenceBrickholizontal(direction, brick2.brickinstance, out rightface2);
        //        referenceArray.Append(rightface2.Reference);
        //        referenceArray1.Append(rightface2.Reference);
        //        var countrowbrick = dic.Keys.Count / space;
        //        var listkeys = dic.Keys.ToList();
        //        for (int i = 1; i < countrowbrick; i++)
        //        {
        //            var value = listkeys.IndexOf(key1);
        //            var value2 = listkeys.IndexOf(key2);
        //            if (value < value2)
        //            {
        //                var key = listkeys[value + i * space];
        //                BrickInformation brick = dic[key].ToList().First();
        //                PlanarFace rightface;
        //                GetReferenceBrickholizontal(direction, brick.brickinstance, out rightface);
        //                referenceArray.Append(rightface.Reference);
        //            }
        //            else
        //            {
        //                var key = listkeys[value2 + i * space];
        //                BrickInformation brick = dic[key].ToList().First();
        //                PlanarFace rightface;
        //                GetReferenceBrickholizontal(direction, brick.brickinstance, out rightface);
        //                referenceArray.Append(rightface.Reference);
        //            }
        //        }
        //        XYZ point3 = sel.PickPoint();
        //        XYZ point4 = sel.PickPoint();
        //        XYZ endp = point3 + direction;
        //        XYZ endp2 = point4 + direction;
        //        Line line = Line.CreateBound(point3, endp);
        //        Line line2 = Line.CreateBound(point4, endp2);
        //        using (Transaction tran = new Transaction(doc, "Create dim brick"))
        //        {
        //            tran.Start();
        //            Dimension dimension = doc.Create.NewDimension(doc.ActiveView, line, referenceArray,dimensionType1);
        //            Dimension dimension1 = doc.Create.NewDimension(doc.ActiveView, line2, referenceArray1, dimensionType2);
        //            AddannotationHolizontal(dimension1, dic);
        //            AddannotationRunningdim(dimension, space);
        //            tran.Commit();
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}
        #endregion
        #region cach2
        public void CreateDimBrickHolizontal(Document doc, Selection sel, int space, DimensionType dimensionType1, DimensionType dimensionType2)
        {
            List<BrickInformation> listbrick = new List<BrickInformation>();
            List<string> Namebricks = new List<string>();
            ReferenceArray referenceArray = new ReferenceArray();
            ReferenceArray referenceArray1 = new ReferenceArray();
            Dictionary<string, List<BrickInformation>> dic1 = Getrowbrick(doc, sel);
            XYZ direction = doc.ActiveView.RightDirection;
            double z;
            List<Dictionary<string, List<BrickInformation>>> listdic = CaculateRowbrick(doc, dic1, sel, direction, ref referenceArray, ref referenceArray1, out z);
            try
            {
                Reference rf = sel.PickObject(ObjectType.Edge);
                referenceArray.Append(rf);
                foreach (var dic in listdic)
                {
                    var countrowbrick = dic.Keys.Count / space;
                    var listkeys = dic.Keys.ToList();
                    for (int i = 1; i < countrowbrick; i++)
                    {
                        var value = listkeys.IndexOf(dic.Keys.First());
                        var key = listkeys[value + i * space];
                        BrickInformation brick = dic[key].ToList().First();
                        PlanarFace rightface;
                        PlanarFace leftface;
                        GetReferenceBrickholizontal(direction, brick.brickinstance, out rightface, out leftface);
                        referenceArray.Append(rightface.Reference);
                    }
                }
                XYZ point3 = sel.PickPoint();
                XYZ point4 = point3 - doc.ActiveView.UpDirection;
                XYZ endp = point3 + direction;
                XYZ endp2 = point4 + direction;
                Line line = Line.CreateBound(point3, endp);
                Line line2 = Line.CreateBound(point4, endp2);
                using (Transaction tran = new Transaction(doc, "Create dim brick"))
                {
                    tran.Start();
                    Dimension dimension = doc.Create.NewDimension(doc.ActiveView, line, referenceArray, dimensionType1);
                    Dimension dimension1 = doc.Create.NewDimension(doc.ActiveView, line2, referenceArray1, dimensionType2);
                    AddannotationHolizontal(dimension1, z);
                    AddannotationRunningdim(dimension, space);
                    tran.Commit();
                }
            }
            catch
            {

            }
        }
        public void CreateDimBrickHolizontal2(Document doc, Selection sel, int space, DimensionType dimensionType1, DimensionType dimensionType2)
        {
            List<BrickInformation> listbrick = new List<BrickInformation>();
            List<string> Namebricks = new List<string>();
            ReferenceArray referenceArray = new ReferenceArray();
            ReferenceArray referenceArray1 = new ReferenceArray();
            Dictionary<string, List<BrickInformation>> dic1 = Getrowbrick2(doc, sel);
            XYZ direction = doc.ActiveView.UpDirection;
            double z;
            List<Dictionary<string, List<BrickInformation>>> listdic = CaculateRowbrick2(doc, dic1, sel, direction, ref referenceArray, ref referenceArray1, out z);
            try
            {
                Reference rf = sel.PickObject(ObjectType.Edge);
                referenceArray.Append(rf);
                foreach (var dic in listdic)
                {
                    var countrowbrick = dic.Keys.Count / space;
                    var listkeys = dic.Keys.ToList();
                    for (int i = 1; i < countrowbrick; i++)
                    {
                        var value = listkeys.IndexOf(dic.Keys.First());
                        var key = listkeys[value + i * space];
                        BrickInformation brick = dic[key].ToList().First();
                        PlanarFace rightface;
                        PlanarFace leftface;
                        GetReferenceBrickholizontal2(direction, brick.brickinstance, out rightface, out leftface);
                        referenceArray.Append(rightface.Reference);
                    }
                }
                XYZ point3 = sel.PickPoint();
                XYZ point4 = point3 - doc.ActiveView.RightDirection;
                XYZ endp = point3 + direction;
                XYZ endp2 = point4 + direction;
                Line line = Line.CreateBound(point3, endp);
                Line line2 = Line.CreateBound(point4, endp2);
                using (Transaction tran = new Transaction(doc, "Create dim brick"))
                {
                    tran.Start();
                    Dimension dimension = doc.Create.NewDimension(doc.ActiveView, line, referenceArray, dimensionType1);
                    Dimension dimension1 = doc.Create.NewDimension(doc.ActiveView, line2, referenceArray1, dimensionType2);
                    AddannotationHolizontal(dimension1, z);
                    AddannotationRunningdim(dimension, space);
                    tran.Commit();
                }
            }
            catch
            {

            }
        }
        public List<Dictionary<string, List<BrickInformation>>> CaculateRowbrick(Document doc, Dictionary<string, List<BrickInformation>> dic, Selection sel, XYZ direction, ref ReferenceArray referenceArray, ref ReferenceArray referenceArray1, out double z)
        {
            List<Dictionary<string, List<BrickInformation>>> listdic = new List<Dictionary<string, List<BrickInformation>>>();
            var val1 = sel.PickObject(ObjectType.Element, new FilterBrick(), "Select brick");
            var val2 = sel.PickObject(ObjectType.Element, new FilterBrick(), "Select brick");
            List<BrickInformation> listvalue = new List<BrickInformation>();
            //rf.ForEach(x => listvalue.Add(new BrickInformation((FamilyInstance)doc.GetElement(x))));
            var brick1 = new BrickInformation((FamilyInstance)doc.GetElement(val1));
            var brick2 = new BrickInformation((FamilyInstance)doc.GetElement(val2));
            listvalue.Add(brick1);
            listvalue.Add(brick2);
            List<string> keylist = new List<string>();
            for (int i = 0; i < listvalue.Count; i++)
            {
                var brick = listvalue[i];
                PlanarFace rightface;
                PlanarFace leftface;
                GetReferenceBrickholizontal(direction, brick.brickinstance, out rightface, out leftface);
                referenceArray.Append(rightface.Reference);
                //if (i == 0)
                //{
                //    referenceArray1.Append(leftface.Reference);
                //}
                //if(i == listvalue.Count - 1)
                //{
                //    referenceArray1.Append(leftface.Reference);
                //}
                //else
                //{
                //    referenceArray1.Append(leftface.Reference);
                //}
            }
            PlanarFace rightface1;
            PlanarFace leftface1;
            GetReferenceBrickholizontal(direction, brick1.brickinstance, out rightface1, out leftface1);
            referenceArray1.Append(leftface1.Reference);
            PlanarFace rightface2;
            PlanarFace leftface2;
            GetReferenceBrickholizontal(direction, brick2.brickinstance, out rightface2, out leftface2);
            referenceArray1.Append(rightface2.Reference);
            var listkey = dic.Keys.ToList();
            foreach (var text in listkey)
            {
                foreach (var item in dic[text])
                {
                    foreach (var item1 in listvalue)
                    {
                        if (int.Parse(item1.ID) == int.Parse(item.ID))
                        {
                            keylist.Add(text);
                        }
                    }
                }
            }
            List<int> Idinstance = new List<int>();
            keylist.ForEach(x => Idinstance.Add(listkey.IndexOf(x)));
            Idinstance.Sort();
            z = Idinstance.Last() - Idinstance.First();
            for (int i = 0; i < Idinstance.Count - 1; i++)
            {
                var j1 = Idinstance[i];
                var j2 = Idinstance[i + 1];
                if (j1 < j2)
                {
                    List<string> list = new List<string>();
                    Dictionary<string, List<BrickInformation>> dic1 = new Dictionary<string, List<BrickInformation>>();
                    for (int k = j1; k < j2; k++)
                    {
                        list.Add(listkey[k]);
                    }
                    list.ForEach(x => dic1.Add(x, dic[x]));
                    if (dic.Keys.Count > 1)
                    {
                        listdic.Add(dic1);
                    }
                }
            }
            return listdic;
        }
        public List<Dictionary<string, List<BrickInformation>>> CaculateRowbrick2(Document doc, Dictionary<string, List<BrickInformation>> dic, Selection sel, XYZ direction, ref ReferenceArray referenceArray, ref ReferenceArray referenceArray1, out double z)
        {
            List<Dictionary<string, List<BrickInformation>>> listdic = new List<Dictionary<string, List<BrickInformation>>>();
            var val1 = sel.PickObject(ObjectType.Element, new FilterBrick(), "Select brick");
            var val2 = sel.PickObject(ObjectType.Element, new FilterBrick(), "Select brick");
            List<BrickInformation> listvalue = new List<BrickInformation>();
            //rf.ForEach(x => listvalue.Add(new BrickInformation((FamilyInstance)doc.GetElement(x))));
            var brick1 = new BrickInformation((FamilyInstance)doc.GetElement(val1));
            var brick2 = new BrickInformation((FamilyInstance)doc.GetElement(val2));
            listvalue.Add(brick1);
            listvalue.Add(brick2);
            List<string> keylist = new List<string>();
            for (int i = 0; i < listvalue.Count; i++)
            {
                var brick = listvalue[i];
                PlanarFace rightface;
                PlanarFace leftface;
                GetReferenceBrickholizontal2(direction, brick.brickinstance, out rightface, out leftface);
                referenceArray.Append(rightface.Reference);
            }
            PlanarFace rightface1;
            PlanarFace leftface1;
            GetReferenceBrickholizontal2(direction, brick1.brickinstance, out rightface1, out leftface1);
            referenceArray1.Append(leftface1.Reference);
            PlanarFace rightface2;
            PlanarFace leftface2;
            GetReferenceBrickholizontal2(direction, brick2.brickinstance, out rightface2, out leftface2);
            referenceArray1.Append(rightface2.Reference);
            var listkey = dic.Keys.ToList();
            foreach (var text in listkey)
            {
                foreach (var item in dic[text])
                {
                    foreach (var item1 in listvalue)
                    {
                        if (int.Parse(item1.ID) == int.Parse(item.ID))
                        {
                            keylist.Add(text);
                        }
                    }
                }
            }
            List<int> Idinstance = new List<int>();
            keylist.ForEach(x => Idinstance.Add(listkey.IndexOf(x)));
            Idinstance.Sort();
            z = Idinstance.Last() + 1;
            for (int i = 0; i < Idinstance.Count - 1; i++)
            {
                var j1 = Idinstance[i];
                var j2 = Idinstance[i + 1];
                if (j1 < j2)
                {
                    List<string> list = new List<string>();
                    Dictionary<string, List<BrickInformation>> dic1 = new Dictionary<string, List<BrickInformation>>();
                    for (int k = j1; k < j2; k++)
                    {
                        list.Add(listkey[k]);
                    }
                    list.ForEach(x => dic1.Add(x, dic[x]));
                    if (dic.Keys.Count > 1)
                    {
                        listdic.Add(dic1);
                    }
                }
            }
            return listdic;
        }
        #endregion

        public void AddannotationHolizontal(Dimension dimension, double z)
        {
            string text = string.Concat(new object[] { z, " ", "BRICKS", " ", "+", " ", z - 1, " ", "JOINTS", "=" });
            dimension.Prefix = text;
        }
        public void AddannotationRunningdim(Dimension dimension, int countrow)
        {
            var countSegment = dimension.Segments.Size;
            for (int i = 1, j = 1; i < countSegment; i++, j += countrow)
            {
                string text = string.Concat(new object[] { "(", j, ")", " ", "COURSES" });
                dimension.Segments.get_Item(i).Below = text;
            }
        }
        Dictionary<string, List<BrickInformation>> Getrowbrick(Document doc, Selection sel)
        {
            Dictionary<string, List<BrickInformation>> dic = SortBrickVertical(doc, sel);
            Dictionary<string, List<BrickInformation>> dic2 = new Dictionary<string, List<BrickInformation>>();
            var listkey = dic.Keys.ToList();
            foreach (var text in listkey)
            {
                var value = dic[text];
                string stringkey = value[0].ControlMark;
                for (int i = 1; i < value.Count; i++)
                {
                    stringkey = stringkey + ";" + value[i].ControlMark;
                }
                var keys = stringkey.Split(';').ToList();
                RemoveContankey(keys);
                var bk = Unionstring(keys) + ";" + text + ";" + value.Count.ToString();
                if (dic2.ContainsKey(bk))
                {
                    dic2[bk].Add((from x in value select x).First());
                }
                else
                {
                    dic2.Add(bk, value);
                }
            }
            return /*filterdicholizontal(dic2)*/ dic2;
        }
        Dictionary<string, List<BrickInformation>> Getrowbrick2(Document doc, Selection sel)
        {
            Dictionary<string, List<BrickInformation>> dic = SortBrickHolizontal2(doc, sel);
            var t1 = Sorteddic(dic);
            Dictionary<string, List<BrickInformation>> dic2 = new Dictionary<string, List<BrickInformation>>();
            var listkey = t1.Keys.ToList();
            foreach (var text in listkey)
            {
                var value = t1[text];
                string stringkey = value[0].ControlMark;
                for (int i = 1; i < value.Count; i++)
                {
                    stringkey = stringkey + ";" + value[i].ControlMark;
                }
                var keys = stringkey.Split(';').ToList();
                RemoveContankey(keys);
                var bk = Unionstring(keys) + ";" + text + ";" + value.Count.ToString();
                if (dic2.ContainsKey(bk))
                {
                    dic2[bk].Add((from x in value select x).First());
                }
                else
                {
                    dic2.Add(bk, value);
                }
            }
            return /*filterdicholizontal(dic2)*/ dic2;
        }
        Dictionary<string, List<BrickInformation>> Sorteddic(Dictionary<string, List<BrickInformation>> dic)
        {
            Dictionary<string, List<BrickInformation>> dic1 = new Dictionary<string, List<BrickInformation>>();
            var list1 = dic.Keys.ToList();
            List<double> listkey = new List<double>();
            list1.ForEach(item => listkey.Add(Convert.ToDouble(item)));
            list1.Sort();
            foreach (var item in list1)
            {
                dic1.Add(item.ToString(), dic[item.ToString()]);
            }
            return dic1;
        }
        Dictionary<string, List<BrickInformation>> filterdicholizontal(Dictionary<string, List<BrickInformation>> dic)
        {
            Dictionary<string, List<BrickInformation>> newdic = new Dictionary<string, List<BrickInformation>>();
            foreach (var text in dic.Keys)
            {
                if (dic[text].Count > 1)
                {
                    newdic.Add(text, dic[text]);
                }
            }
            return newdic;
        }
        public void AddAnotationVertical(Dimension dimension, string keytext, Dictionary<string, List<BrickInformation>> dic)
        {
            var countsegment = dimension.Segments.Size;
            var typebricks = keytext.Split(';').ToList().First().Split('-').ToList();
            if (countsegment == 2)
            {
                string text1 = typebricks.First();
                string text2 = string.Concat(new object[] { "(", dic[keytext].Count() - 1, ")", " ", typebricks[1], " ", "+", " ", "(", dic[keytext].Count() - 1, ")", " ", "JST." });
                dimension.Segments.get_Item(0).Below = text1;
                dimension.Segments.get_Item(1).Below = text2;
            }
            if (countsegment == 3)
            {
                string text1 = typebricks.First();
                string text2 = string.Concat(new object[] { "(", dic[keytext].Count() - 2, ")", " ", typebricks[1], " ", "+", " ", "(", dic[keytext].Count() - 1, ")", " ", "JST." });

                string text3 = typebricks.Last();
                dimension.Segments.get_Item(0).Below = text3;
                dimension.Segments.get_Item(1).Below = text2;
                dimension.Segments.get_Item(2).Below = text1;
            }
        }
        public void AddAnotationVertical2(Dimension dimension, string keytext, Dictionary<string, List<BrickInformation>> dic)
        {
            var countsegment = dimension.Segments.Size;
            var typebricks = keytext.Split(';').ToList().First().Split('-').ToList();
            if (countsegment == 2)
            {
                string text1 = typebricks.First();
                string text2 = string.Concat(new object[] { "(", dic[keytext].Count() - 1, ")", " ", typebricks[1], " ", "+", " ", "(", dic[keytext].Count() - 1, ")", " ", "JST." });
                dimension.Segments.get_Item(0).Below = text1;
                dimension.Segments.get_Item(1).Below = text2;
            }
            if (countsegment == 3)
            {
                if (typebricks.Count == 3)
                {
                    string text1 = typebricks.First();
                    string text2 = string.Concat(new object[] { "(", dic[keytext].Count() - 2, ")", " ", typebricks[1], " ", "+", " ", "(", dic[keytext].Count() - 1, ")", " ", "JST." });
                    string text3 = typebricks.Last();
                    dimension.Segments.get_Item(0).Below = text3;
                    dimension.Segments.get_Item(1).Below = text2;
                    dimension.Segments.get_Item(2).Below = text1;
                }
                if (typebricks.Count == 1)
                {
                    string text1 = typebricks.First();
                    string text2 = string.Concat(new object[] { "(", dic[keytext].Count() - 2, ")", " ", typebricks.First(), " ", "+", " ", "(", dic[keytext].Count() - 1, ")", " ", "JST." });
                    string text3 = typebricks.Last();
                    dimension.Segments.get_Item(0).Below = text3;
                    dimension.Segments.get_Item(1).Below = text2;
                    dimension.Segments.get_Item(2).Below = text1;
                }
                else
                {
                    string text1 = typebricks.First();
                    //string text2 = string.Concat(new object[] { "(", dic[keytext].Count() - 2, ")", " ", typebricks[1], " ", "+", " ", "(", dic[keytext].Count() - 1, ")", " ", "JST." });
                    string text3 = typebricks.Last();
                    dimension.Segments.get_Item(0).Below = text3;
                    dimension.Segments.get_Item(2).Below = text1;
                }
            }
            if (countsegment > 3)
            {
                string text1 = typebricks.First();
                string text2 = string.Concat(new object[] { "(", dic[keytext].Count() - 2, ")", " ", typebricks[1], " ", "+", " ", "(", dic[keytext].Count() - 1, ")", " ", "JST." });
                string text3 = typebricks.Last();
                dimension.Segments.get_Item(0).Below = text3;

                dimension.Segments.get_Item(countsegment - 1).Below = text1;
            }
        }
        public void GetReferenceBrick(XYZ direction, FamilyInstance brick, out PlanarFace rightFace, out PlanarFace leftFace)
        {
            XYZ location = ((LocationPoint)brick.Location).Point;
            IList<PlanarFace> Listfaces = FaceofBrick(brick);
            Transform transform = brick.GetTransform();
            rightFace = Facexanhat(location, direction, Listfaces, transform);
            leftFace = Facexanhat(location, -direction, Listfaces, transform);
        }
        public void GetReferenceBrickholizontal(XYZ direction, FamilyInstance brick, out PlanarFace rightFace, out PlanarFace leftFace)
        {
            XYZ location = ((LocationPoint)brick.Location).Point;
            IList<PlanarFace> Listfaces = FaceofBrick(brick);
            Transform transform = brick.GetTransform();
            rightFace = Facegannhat(location, -direction, Listfaces, transform);
            leftFace = Facexanhat(location, direction, Listfaces, transform);
        }
        public void GetReferenceBrickholizontal2(XYZ direction, FamilyInstance brick, out PlanarFace rightFace, out PlanarFace leftFace)
        {
            XYZ location = ((LocationPoint)brick.Location).Point;
            IList<PlanarFace> Listfaces = FaceofBrick(brick);
            Transform transform = brick.GetTransform();
            rightFace = Facegannhat(location, -direction, Listfaces, transform);
            leftFace = Facexanhat(location, direction, Listfaces, transform);
        }
        public PlanarFace Facexanhat(XYZ point, XYZ direction, IList<PlanarFace> listFaces, Transform transform)
        {
            PlanarFace planarFace = null;
            double num = double.MinValue;
            PLane3D plane3D = new PLane3D(point, direction);
            foreach (PlanarFace i in listFaces)
            {
                XYZ xyz = transform.OfVector(i.FaceNormal);
                XYZ xyz2 = direction.CrossProduct(xyz);
                bool check = xyz2.GetLength() > 0.001 || xyz.DotProduct(direction) < 0.0;
                if (!check)
                {
                    double num2 = plane3D.DistancepointtoPlane(transform.OfPoint(i.Origin));
                    bool flag2 = num2 < 0.001;
                    if (!flag2)
                    {
                        bool flag3 = num2 > num;
                        if (flag3)
                        {
                            planarFace = i;
                            num = num2;
                        }
                    }
                }
            }
            return planarFace;
        }
        public void CreateTypetextnote(Document doc)
        {
            try
            {
                var colec = new FilteredElementCollector(doc).OfClass(typeof(TextNoteType)).Cast<TextNoteType>().ToList();
                if (!checktypetext(colec))
                {
                    using (Transaction tran = new Transaction(doc, "Create new type text"))
                    {
                        tran.Start();
                        TextNoteType ele = (TextNoteType)colec.First().Duplicate("text brick");
                        if (ele != null)
                        {
                            (from x in ele.ParametersMap.Cast<Parameter>().ToList() where x.Definition.Name == "Text Font" select x).Cast<Parameter>().First().Set("Arial Narrow");
                            (from x in ele.ParametersMap.Cast<Parameter>().ToList() where x.Definition.Name == "Text Size" select x).Cast<Parameter>().First().Set("0.0025");
                        }
                        tran.Commit();
                    }
                }
            }
            catch
            {
                var colec = new FilteredElementCollector(doc).OfClass(typeof(TextNoteType)).Cast<TextNoteType>().ToList();
                using (Transaction tran = new Transaction(doc, "Create new type text"))
                {
                    tran.Start();
                    TextNoteType ele = (TextNoteType)colec.First().Duplicate("text brick");
                    if (ele != null)
                    {
                        (from x in ele.ParametersMap.Cast<Parameter>().ToList() where x.Definition.Name == "Text Font" select x).Cast<Parameter>().First().Set("Arial Narrow");
                        (from x in ele.ParametersMap.Cast<Parameter>().ToList() where x.Definition.Name == "Text Size" select x).Cast<Parameter>().First().Set("0.25");
                    }
                    tran.Commit();
                }
            }

        }
        bool checktypetext(List<TextNoteType> textNoteTypes)
        {
            bool flag = true;
            var bn = (from TextNoteType x in textNoteTypes where x.Name.Equals("text brick") select x).Cast<TextNoteType>().First();
            if (bn != null)
            {
                flag = true;
            }
            else
            {
                flag = false;
            }
            return flag;
        }
        public PlanarFace Facegannhat(XYZ point, XYZ direction, IList<PlanarFace> listFaces, Transform transform)
        {
            PlanarFace planarFace = null;
            double num = double.MinValue;
            PLane3D plane3D = new PLane3D(point, direction);
            foreach (PlanarFace i in listFaces)
            {
                XYZ xyz = transform.OfVector(i.FaceNormal);
                XYZ xyz2 = direction.CrossProduct(xyz);
                bool check = xyz2.GetLength() > 0.001 || xyz.DotProduct(direction) < 0.0;
                if (!check)
                {
                    double num2 = plane3D.DistancepointtoPlane(transform.OfPoint(i.Origin));
                    bool flag2 = num2 < 0.001;
                    if (!flag2)
                    {
                        bool flag3 = num2 > num;
                        if (flag3)
                        {
                            planarFace = i;
                            num = num2;
                        }
                    }
                }
            }
            return planarFace;
        }
        public IList<PlanarFace> FaceofBrick(FamilyInstance familyInstance)
        {
            Options options = new Options();
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = true;
            options.DetailLevel = ViewDetailLevel.Undefined;
            string name = familyInstance.Name;
            IList<PlanarFace> list = new List<PlanarFace>();
            GeometryElement geometryElement = familyInstance.get_Geometry(options);
            bool flag = geometryElement == null;
            IList<PlanarFace> result;
            if (flag)
            {
                result = list;
            }
            else
            {
                foreach (GeometryObject geometryObject in geometryElement)
                {
                    GeometryInstance geometryInstance = geometryObject as GeometryInstance;
                    bool flag2 = null != geometryInstance;
                    if (flag2)
                    {
                        GeometryElement symbolGeometry = geometryInstance.GetSymbolGeometry();
                        foreach (GeometryObject geometryObject2 in symbolGeometry)
                        {
                            Solid solid = geometryObject2 as Solid;
                            bool flag3 = null == solid || solid.Faces.Size == 0 || solid.Edges.Size == 0;
                            if (!flag3)
                            {
                                foreach (object obj in solid.Faces)
                                {
                                    Face face = (Face)obj;
                                    PlanarFace planarFace = face as PlanarFace;
                                    bool flag4 = planarFace != null;
                                    if (flag4)
                                    {
                                        list.Add(planarFace);
                                    }
                                }
                            }
                        }
                    }
                }
                result = list;
            }
            return result;
        }
        public List<DimensionType> GetDimensions(Document doc)
        {
            var col = new FilteredElementCollector(doc).OfClass(typeof(DimensionType)).Cast<DimensionType>().ToList();
            var list = (from x in col where x.StyleType == DimensionStyleType.Linear orderby x.Name select x).ToList();
            return list;
        }
    }
    public class BrickInformation
    {
        public string ControlMark { get; set; }
        public string ID { get; set; }
        public XYZ Location { get; set; }
        public FamilyInstance brickinstance { get; }
        public Transform Transform { get; }
        public BrickInformation(FamilyInstance familyInstance)
        {
            ID = familyInstance.Id.ToString();
            ControlMark = familyInstance.LookupParameter("BRICK_NAME").AsString();
            Transform = familyInstance.GetTransform();
            Location = ((LocationPoint)familyInstance.Location).Point;
            brickinstance = familyInstance;
        }
    }
    public class SettingBrick
    {
        private static SettingBrick _instance;
        private SettingBrick()
        {

        }
        public static SettingBrick Instance => _instance ?? (_instance = new SettingBrick());
        public string DimensionVertical { get; set; }
        public string Dimensiontypeholizontal1 { get; set; }
        public string Dimensiontypeholizontal2 { get; set; }
        public string Space { get; set; }
        public string GetFolderPath()
        {
            string folderPath = SettingExtension.GetSettingPath() + "\\TVD.11.SettingBrick";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public string GetFileName()
        {
            return "SettingBrick.json";
        }

        public string GetFullFileName()
        {
            return GetFolderPath() + "\\" + GetFileName();
        }

        public void SaveSetting()
        {
            var gh = GetFullFileName();
            SettingExtension.SaveSetting(this, GetFullFileName());
        }

        public SettingBrick GetSetting()
        {
            SettingBrick setting = SettingExtension.GetSetting<SettingBrick>(GetFullFileName());
            if (setting == null) setting = new SettingBrick();
            return setting;
        }
    }
}
