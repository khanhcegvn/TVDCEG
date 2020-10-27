using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using c = Autodesk.Revit.ApplicationServices;


namespace TVDCEG
{
    public class lbr
    {
        public void RemoveShareParameter(Document doc, ref Dictionary<string, string> dic)
        {
            IList<FamilyParameter> list = new List<FamilyParameter>();
            FamilyManager familyManager = doc.FamilyManager;
            list = familyManager.GetParameters();
            Transaction newtran = new Transaction(doc, "s");
            newtran.Start();
            var bm = familyManager.CurrentType;
            foreach (var i in list)
            {
                if (!i.Definition.ParameterGroup.ToString().Equals("PG_CONSTRAINTS") && !i.Definition.ParameterGroup.ToString().Equals("PG_IDENTITY_DATA"))
                {
                    if(i.StorageType == StorageType.String)
                    {
                        string value = bm.AsString(i);
                        dic.Add(i.Definition.Name, value);
                    }
                    if(i.StorageType == StorageType.Double)
                    {
                        string value = bm.AsDouble(i).ToString();
                        dic.Add(i.Definition.Name, value);
                    }
                    familyManager.RemoveParameter(i);
                }
            }
            newtran.Commit();
        }
        public void RemoveShareParameterkeep(Document doc)
        {
            IList<FamilyParameter> list = new List<FamilyParameter>();
            FamilyManager familyManager = doc.FamilyManager;
            list = familyManager.GetParameters();
            Transaction newtran = new Transaction(doc, "s");
            newtran.Start();
            foreach (var i in list)
            {
                if (!i.Definition.ParameterGroup.ToString().Equals("PG_CONSTRAINTS") && !i.Definition.ParameterGroup.ToString().Equals("PG_IDENTITY_DATA") && !i.Definition.ParameterGroup.ToString().Equals("PG_GEOMETRY") && !i.Definition.ParameterGroup.ToString().Equals("PG_DATA"))
                {
                    familyManager.RemoveParameter(i);
                }
            }
            newtran.Commit();
        }
        public FamilyParameter _AddParameter(Document doc, DefinitionGroup i, string Nameitem, BuiltInParameterGroup builtInParameterGroup, bool po)
        {
            FamilyManager familyManager = doc.FamilyManager;
            ExternalDefinition p = i.Definitions.get_Item(Nameitem) as ExternalDefinition;
            FamilyParameter pa = familyManager.AddParameter(p, builtInParameterGroup, po);
            return pa;
        }
        public void EmbedStandard(Document doc, c.Application app, Dictionary<string, string> dic)
        {
            FamilyManager familyManager = doc.FamilyManager;
            string origfile = app.SharedParametersFilename;
            string tempFile = @"C:\ProgramData\Autodesk\ApplicationPlugins\TVD\Shared_Params_2015_v01.txt";
            Transaction tran = new Transaction(doc, "add parameter");
            tran.Start();
            app.SharedParametersFilename = tempFile;
            DefinitionFile shareParameterfile = app.OpenSharedParameterFile();
            foreach (DefinitionGroup i in shareParameterfile.Groups)
            {
                if (i.Name == "IDENTITY")
                {
                    //group data
                    FamilyParameter p1 = _AddParameter(doc, i, "CONTROL_MARK", BuiltInParameterGroup.PG_DATA, false);
                    string vp1 = Getvalueparameter("CONTROL_MARK", dic);
                    familyManager.Set(p1, vp1);
                    FamilyParameter p2 = _AddParameter(doc, i, "IDENTITY_COMMENT", BuiltInParameterGroup.PG_DATA, false);
                    string vp2 = Getvalueparameter("IDENTITY_COMMENT", dic);
                    familyManager.Set(p2, vp2);
                    FamilyParameter p3 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    string vp3 = Getvalueparameter("IDENTITY_DESCRIPTION", dic);
                    familyManager.Set(p3, vp3);
                    FamilyParameter p4 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION_SHORT", BuiltInParameterGroup.PG_DATA, false);
                    string vp4 = Getvalueparameter("IDENTITY_DESCRIPTION_SHORT", dic);
                    familyManager.Set(p4, vp4);
                    FamilyParameter p5 = _AddParameter(doc, i, "MANUFACTURER_PLANT_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    string vp5 = Getvalueparameter("MANUFACTURER_PLANT_DESCRIPTION", dic);
                    familyManager.Set(p5, vp5);
                    FamilyParameter p6 = _AddParameter(doc, i, "MANUFACTURER_PLANT_ID", BuiltInParameterGroup.PG_DATA, false);
                    string vp6 = Getvalueparameter("MANUFACTURER_PLANT_ID", dic);
                    familyManager.Set(p6, vp6);
                    FamilyParameter p7 = _AddParameter(doc, i, "MANUFACTURE_COMPONENT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p8 = _AddParameter(doc, i, "SORTING_ORDER", BuiltInParameterGroup.PG_DATA, false);
                    familyManager.Set(p7, "EMBED STANDARD");
                    familyManager.Set(p8, int.Parse("100"));
                }
                if (i.Name == "DIMENSIONS_GENERAL")
                {
                    FamilyParameter p1 = _AddParameter(doc, i, "DIM_HEIGHT", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp1 = Getvalueparameter("DIM_HEIGHT", dic);
                    if (!string.IsNullOrEmpty(vp1))
                    {
                        familyManager.Set(p1, Convert.ToDouble(vp1));
                    }
                    FamilyParameter p2 = _AddParameter(doc, i, "DIM_THICKNESS", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp2 = Getvalueparameter("DIM_THICKNESS", dic);
                    if (!string.IsNullOrEmpty(vp2))
                    {
                        familyManager.Set(p2, Convert.ToDouble(vp2));
                    }
                    FamilyParameter p3 = _AddParameter(doc, i, "DIM_WIDTH", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp3 = Getvalueparameter("DIM_WIDTH", dic);
                    if (!string.IsNullOrEmpty(vp3))
                    {
                        familyManager.Set(p3, Convert.ToDouble(vp3));
                    }
                    FamilyParameter _p3 = _AddParameter(doc, i, "DBA_Length", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string _vp3 = Getvalueparameter("DBA_Length", dic);
                    if (!string.IsNullOrEmpty(_vp3))
                    {
                        familyManager.Set(_p3, Convert.ToDouble(_vp3));
                    }
                }
                //if (i.Name == "FINISHES")
                //{
                //    FamilyParameter p1 = _AddParameter(doc, i, "FINISH_BLACK", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p2 = _AddParameter(doc, i, "FINISH_GALVANIZED", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p3 = _AddParameter(doc, i, "FINISH_OTHER", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p4 = _AddParameter(doc, i, "FINISH_PAINTED", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p5 = _AddParameter(doc, i, "FINISH_PRIMED", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p6 = _AddParameter(doc, i, "FINISH_STAINLESS", BuiltInParameterGroup.PG_MATERIALS, false);
                //}

            }
            tran.Commit();

        }
        public void EmbedStandardkeepdata(Document doc, c.Application app, Dictionary<string, string> dic)
        {
            FamilyManager familyManager = doc.FamilyManager;
            string origfile = app.SharedParametersFilename;
            string tempFile = @"C:\ProgramData\Autodesk\ApplicationPlugins\TVD\Shared_Params_2015_v01.txt";
            Transaction tran = new Transaction(doc, "add parameter");
            tran.Start();
            app.SharedParametersFilename = tempFile;
            DefinitionFile shareParameterfile = app.OpenSharedParameterFile();
            foreach (DefinitionGroup i in shareParameterfile.Groups)
            {
                if (i.Name == "IDENTITY")
                {
                    //group data
                    FamilyParameter p1 = _AddParameter(doc, i, "CONTROL_MARK", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p2 = _AddParameter(doc, i, "IDENTITY_COMMENT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p3 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p4 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION_SHORT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p5 = _AddParameter(doc, i, "MANUFACTURER_PLANT_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p6 = _AddParameter(doc, i, "MANUFACTURER_PLANT_ID", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p7 = _AddParameter(doc, i, "MANUFACTURE_COMPONENT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p8 = _AddParameter(doc, i, "SORTING_ORDER", BuiltInParameterGroup.PG_DATA, false);
                    familyManager.Set(p7, "EMBED STANDARD");
                    familyManager.Set(p8, int.Parse("100"));
                }
                if (i.Name == "DIMENSIONS_GENERAL")
                {
                    FamilyParameter p1 = _AddParameter(doc, i, "DIM_HEIGHT", BuiltInParameterGroup.PG_GEOMETRY, false);
                    FamilyParameter p2 = _AddParameter(doc, i, "DIM_THICKNESS", BuiltInParameterGroup.PG_GEOMETRY, false);
                    FamilyParameter p3 = _AddParameter(doc, i, "DIM_WIDTH", BuiltInParameterGroup.PG_GEOMETRY, false);
                }
                if (i.Name == "FINISHES")
                {
                    FamilyParameter p1 = _AddParameter(doc, i, "FINISH_BLACK", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p2 = _AddParameter(doc, i, "FINISH_GALVANIZED", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p3 = _AddParameter(doc, i, "FINISH_OTHER", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p4 = _AddParameter(doc, i, "FINISH_PAINTED", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p5 = _AddParameter(doc, i, "FINISH_PRIMED", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p6 = _AddParameter(doc, i, "FINISH_STAINLESS", BuiltInParameterGroup.PG_MATERIALS, false);
                }

            }
            tran.Commit();

        }
        public void EmbedStandard_finish(Document doc, c.Application app, Dictionary<string, string> dic)
        {

            FamilyManager familyManager = doc.FamilyManager;
            string origfile = app.SharedParametersFilename;
            string tempFile = @"C:\ProgramData\Autodesk\ApplicationPlugins\TVD\Shared_Params_2015_v01.txt";
            Transaction tran = new Transaction(doc, "add parameter");
            tran.Start();
            app.SharedParametersFilename = tempFile;
            DefinitionFile shareParameterfile = app.OpenSharedParameterFile();
            FamilyParameter p10 = doc.FamilyManager.AddParameter("desc_b", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p11 = doc.FamilyManager.AddParameter("desc_g", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p12 = doc.FamilyManager.AddParameter("desc_o", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p13 = doc.FamilyManager.AddParameter("desc_pr", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p14 = doc.FamilyManager.AddParameter("desc_pt", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p15 = doc.FamilyManager.AddParameter("desc_s", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p16 = doc.FamilyManager.AddParameter("finish_check", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p17 = doc.FamilyManager.AddParameter("manufacturer_plant_description_b", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p18 = doc.FamilyManager.AddParameter("manufacturer_plant_description_g", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p19 = doc.FamilyManager.AddParameter("manufacturer_plant_description_o", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p20 = doc.FamilyManager.AddParameter("manufacturer_plant_description_pr", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p21 = doc.FamilyManager.AddParameter("manufacturer_plant_description_pt", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p22 = doc.FamilyManager.AddParameter("manufacturer_plant_description_s", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p23 = doc.FamilyManager.AddParameter("manufacturer_plant_id_b", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p24 = doc.FamilyManager.AddParameter("manufacturer_plant_id_g", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p25 = doc.FamilyManager.AddParameter("manufacturer_plant_id_o", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p26 = doc.FamilyManager.AddParameter("manufacturer_plant_id_pr", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p27 = doc.FamilyManager.AddParameter("manufacturer_plant_id_pt", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p28 = doc.FamilyManager.AddParameter("manufacturer_plant_id_s", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p29 = doc.FamilyManager.AddParameter("mark_b", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p30 = doc.FamilyManager.AddParameter("mark_g", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p31 = doc.FamilyManager.AddParameter("mark_o", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p32 = doc.FamilyManager.AddParameter("mark_pr", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p33 = doc.FamilyManager.AddParameter("mark_pt", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p34 = doc.FamilyManager.AddParameter("mark_s", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            foreach (DefinitionGroup i in shareParameterfile.Groups)
            {
                if (i.Name == "IDENTITY")
                {
                    //group data
                    FamilyParameter p1 = _AddParameter(doc, i, "CONTROL_MARK", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p2 = _AddParameter(doc, i, "IDENTITY_COMMENT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p3 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p4 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION_SHORT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p5 = _AddParameter(doc, i, "MANUFACTURER_PLANT_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p6 = _AddParameter(doc, i, "MANUFACTURER_PLANT_ID", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p7 = _AddParameter(doc, i, "MANUFACTURE_COMPONENT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p8 = _AddParameter(doc, i, "SORTING_ORDER", BuiltInParameterGroup.PG_DATA, false);
                    familyManager.SetFormula(p1, "if(FINISH_GALVANIZED, mark_g, (if(FINISH_PAINTED, mark_pt, (if(FINISH_PRIMED, mark_pr, (if(FINISH_OTHER, mark_o, (if(FINISH_STAINLESS, mark_s, mark_b)))))))))");
                    familyManager.SetFormula(p2, "if(FINISH_GALVANIZED, desc_g, (if(FINISH_PAINTED, desc_pt, (if(FINISH_PRIMED, desc_pr, (if(FINISH_OTHER, desc_o, (if(FINISH_STAINLESS, desc_s, desc_b)))))))))");
                    familyManager.SetFormula(p3, "if(FINISH_GALVANIZED, mark_g, (if(FINISH_PAINTED, mark_pt, (if(FINISH_PRIMED, mark_pr, (if(FINISH_OTHER, mark_o, (if(FINISH_STAINLESS, mark_s, mark_b)))))))))");
                    familyManager.SetFormula(p4, "if(FINISH_GALVANIZED, manufacturer_plant_description_g, (if(FINISH_PAINTED, manufacturer_plant_description_pt, (if(FINISH_PRIMED, manufacturer_plant_description_pr, (if(FINISH_OTHER, manufacturer_plant_description_o, (if(FINISH_STAINLESS, manufacturer_plant_description_s, manufacturer_plant_description_b)))))))))");
                    familyManager.SetFormula(p5, "if(FINISH_GALVANIZED, manufacturer_plant_id_g, (if(FINISH_PAINTED, manufacturer_plant_id_pt, (if(FINISH_PRIMED, manufacturer_plant_id_pr, (if(FINISH_OTHER, manufacturer_plant_id_o, (if(FINISH_STAINLESS, manufacturer_plant_id_s, manufacturer_plant_id_b)))))))))");
                    familyManager.Set(p7, "EMBED STANDARD");
                    familyManager.Set(p8, int.Parse("100"));
                }
                if (i.Name == "DIMENSIONS_GENERAL")
                {
                    FamilyParameter p1 = _AddParameter(doc, i, "DIM_HEIGHT", BuiltInParameterGroup.PG_GEOMETRY, false);
                    FamilyParameter p2 = _AddParameter(doc, i, "DIM_THICKNESS", BuiltInParameterGroup.PG_GEOMETRY, false);
                    FamilyParameter p3 = _AddParameter(doc, i, "DIM_WIDTH", BuiltInParameterGroup.PG_GEOMETRY, false);
                }
                if (i.Name == "FINISHES")
                {
                    FamilyParameter p1 = _AddParameter(doc, i, "FINISH_BLACK", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p2 = _AddParameter(doc, i, "FINISH_GALVANIZED", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p3 = _AddParameter(doc, i, "FINISH_OTHER", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p4 = _AddParameter(doc, i, "FINISH_PAINTED", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p5 = _AddParameter(doc, i, "FINISH_PRIMED", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p6 = _AddParameter(doc, i, "FINISH_STAINLESS", BuiltInParameterGroup.PG_MATERIALS, false);

                }

            }
            tran.Commit();
        }
        public void EmbedCustom(Document doc, c.Application app, Dictionary<string, string> dic)
        {
            FamilyManager familyManager = doc.FamilyManager;
            string origfile = app.SharedParametersFilename;
            string tempFile = @"C:\ProgramData\Autodesk\ApplicationPlugins\TVD\Shared_Params_2015_v01.txt";
            Transaction tran = new Transaction(doc, "add parameter");
            tran.Start();
            app.SharedParametersFilename = tempFile;
            DefinitionFile shareParameterfile = app.OpenSharedParameterFile();
            foreach (DefinitionGroup i in shareParameterfile.Groups)
            {
                if (i.Name == "IDENTITY")
                {
                    //group data
                    FamilyParameter p1 = _AddParameter(doc, i, "CONTROL_MARK", BuiltInParameterGroup.PG_DATA, false);
                    string vp1 = Getvalueparameter("CONTROL_MARK", dic);
                    familyManager.Set(p1, vp1);
                    FamilyParameter p2 = _AddParameter(doc, i, "IDENTITY_COMMENT", BuiltInParameterGroup.PG_DATA, false);
                    string vp2 = Getvalueparameter("IDENTITY_COMMENT", dic);
                    familyManager.Set(p2, vp2);
                    FamilyParameter p3 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    string vp3 = Getvalueparameter("IDENTITY_DESCRIPTION", dic);
                    familyManager.Set(p3, vp3);
                    FamilyParameter p4 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION_SHORT", BuiltInParameterGroup.PG_DATA, false);
                    string vp4 = Getvalueparameter("IDENTITY_DESCRIPTION_SHORT", dic);
                    familyManager.Set(p4, vp4);
                    FamilyParameter p5 = _AddParameter(doc, i, "MANUFACTURER_PLANT_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    string vp5 = Getvalueparameter("MANUFACTURER_PLANT_DESCRIPTION", dic);
                    familyManager.Set(p5, vp5);
                    FamilyParameter p6 = _AddParameter(doc, i, "MANUFACTURER_PLANT_ID", BuiltInParameterGroup.PG_DATA, false);
                    string vp6 = Getvalueparameter("MANUFACTURER_PLANT_ID", dic);
                    familyManager.Set(p6, vp6);
                    FamilyParameter p7 = _AddParameter(doc, i, "MANUFACTURE_COMPONENT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p8 = _AddParameter(doc, i, "SORTING_ORDER", BuiltInParameterGroup.PG_DATA, false);
                    familyManager.Set(p7, "EMBED CUSTOM");
                    familyManager.Set(p8, int.Parse("102"));
                }
                if (i.Name == "DIMENSIONS_GENERAL")
                {
                    FamilyParameter p1 = _AddParameter(doc, i, "DIM_HEIGHT", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp1 = Getvalueparameter("DIM_HEIGHT", dic);
                    if (!string.IsNullOrEmpty(vp1))
                    {
                        familyManager.Set(p1, Convert.ToDouble(vp1));
                    }
                    FamilyParameter p2 = _AddParameter(doc, i, "DIM_THICKNESS", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp2 = Getvalueparameter("DIM_THICKNESS", dic);
                    if (!string.IsNullOrEmpty(vp2))
                    {
                        familyManager.Set(p2, Convert.ToDouble(vp2));
                    }
                    FamilyParameter p3 = _AddParameter(doc, i, "DIM_WIDTH", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp3 = Getvalueparameter("DIM_WIDTH", dic);
                    if (!string.IsNullOrEmpty(vp3))
                    {
                        familyManager.Set(p3, Convert.ToDouble(vp3));
                    }
                    FamilyParameter _p3 = _AddParameter(doc, i, "DBA_Length", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string _vp3 = Getvalueparameter("DBA_Length", dic);
                    if (!string.IsNullOrEmpty(_vp3))
                    {
                        familyManager.Set(_p3, Convert.ToDouble(_vp3));
                    }
                }
                //if (i.Name == "FINISHES")
                //{
                //    FamilyParameter p1 = _AddParameter(doc, i, "FINISH_BLACK", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p2 = _AddParameter(doc, i, "FINISH_GALVANIZED", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p3 = _AddParameter(doc, i, "FINISH_OTHER", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p4 = _AddParameter(doc, i, "FINISH_PAINTED", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p5 = _AddParameter(doc, i, "FINISH_PRIMED", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p6 = _AddParameter(doc, i, "FINISH_STAINLESS", BuiltInParameterGroup.PG_MATERIALS, false);
                //}

            }
            tran.Commit();

        }

        public void EmbedCustom_finish(Document doc, c.Application app, Dictionary<string, string> dic)
        {

            FamilyManager familyManager = doc.FamilyManager;
            string origfile = app.SharedParametersFilename;
            string tempFile = @"C:\ProgramData\Autodesk\ApplicationPlugins\TVD\Shared_Params_2015_v01.txt";
            Transaction tran = new Transaction(doc, "add parameter");
            tran.Start();
            app.SharedParametersFilename = tempFile;
            DefinitionFile shareParameterfile = app.OpenSharedParameterFile();
            FamilyParameter p10 = doc.FamilyManager.AddParameter("desc_b", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p11 = doc.FamilyManager.AddParameter("desc_g", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p12 = doc.FamilyManager.AddParameter("desc_o", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p13 = doc.FamilyManager.AddParameter("desc_pr", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p14 = doc.FamilyManager.AddParameter("desc_pt", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p15 = doc.FamilyManager.AddParameter("desc_s", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p16 = doc.FamilyManager.AddParameter("finish_check", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p17 = doc.FamilyManager.AddParameter("manufacturer_plant_description_b", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p18 = doc.FamilyManager.AddParameter("manufacturer_plant_description_g", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p19 = doc.FamilyManager.AddParameter("manufacturer_plant_description_o", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p20 = doc.FamilyManager.AddParameter("manufacturer_plant_description_pr", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p21 = doc.FamilyManager.AddParameter("manufacturer_plant_description_pt", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p22 = doc.FamilyManager.AddParameter("manufacturer_plant_description_s", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p23 = doc.FamilyManager.AddParameter("manufacturer_plant_id_b", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p24 = doc.FamilyManager.AddParameter("manufacturer_plant_id_g", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p25 = doc.FamilyManager.AddParameter("manufacturer_plant_id_o", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p26 = doc.FamilyManager.AddParameter("manufacturer_plant_id_pr", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p27 = doc.FamilyManager.AddParameter("manufacturer_plant_id_pt", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p28 = doc.FamilyManager.AddParameter("manufacturer_plant_id_s", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p29 = doc.FamilyManager.AddParameter("mark_b", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p30 = doc.FamilyManager.AddParameter("mark_g", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p31 = doc.FamilyManager.AddParameter("mark_o", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p32 = doc.FamilyManager.AddParameter("mark_pr", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p33 = doc.FamilyManager.AddParameter("mark_pt", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p34 = doc.FamilyManager.AddParameter("mark_s", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            foreach (DefinitionGroup i in shareParameterfile.Groups)
            {
                if (i.Name == "IDENTITY")
                {
                    //group data
                    FamilyParameter p1 = _AddParameter(doc, i, "CONTROL_MARK", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p2 = _AddParameter(doc, i, "IDENTITY_COMMENT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p3 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p4 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION_SHORT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p5 = _AddParameter(doc, i, "MANUFACTURER_PLANT_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p6 = _AddParameter(doc, i, "MANUFACTURER_PLANT_ID", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p7 = _AddParameter(doc, i, "MANUFACTURE_COMPONENT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p8 = _AddParameter(doc, i, "SORTING_ORDER", BuiltInParameterGroup.PG_DATA, false);
                    familyManager.SetFormula(p1, "if(FINISH_GALVANIZED, mark_g, (if(FINISH_PAINTED, mark_pt, (if(FINISH_PRIMED, mark_pr, (if(FINISH_OTHER, mark_o, (if(FINISH_STAINLESS, mark_s, mark_b)))))))))");
                    familyManager.SetFormula(p2, "if(FINISH_GALVANIZED, desc_g, (if(FINISH_PAINTED, desc_pt, (if(FINISH_PRIMED, desc_pr, (if(FINISH_OTHER, desc_o, (if(FINISH_STAINLESS, desc_s, desc_b)))))))))");
                    familyManager.SetFormula(p3, "if(FINISH_GALVANIZED, mark_g, (if(FINISH_PAINTED, mark_pt, (if(FINISH_PRIMED, mark_pr, (if(FINISH_OTHER, mark_o, (if(FINISH_STAINLESS, mark_s, mark_b)))))))))");
                    familyManager.SetFormula(p4, "if(FINISH_GALVANIZED, manufacturer_plant_description_g, (if(FINISH_PAINTED, manufacturer_plant_description_pt, (if(FINISH_PRIMED, manufacturer_plant_description_pr, (if(FINISH_OTHER, manufacturer_plant_description_o, (if(FINISH_STAINLESS, manufacturer_plant_description_s, manufacturer_plant_description_b)))))))))");
                    familyManager.SetFormula(p5, "if(FINISH_GALVANIZED, manufacturer_plant_id_g, (if(FINISH_PAINTED, manufacturer_plant_id_pt, (if(FINISH_PRIMED, manufacturer_plant_id_pr, (if(FINISH_OTHER, manufacturer_plant_id_o, (if(FINISH_STAINLESS, manufacturer_plant_id_s, manufacturer_plant_id_b)))))))))");
                    familyManager.Set(p7, "EMBED CUSTOM");
                    familyManager.Set(p8, int.Parse("102"));
                }
                if (i.Name == "DIMENSIONS_GENERAL")
                {
                    FamilyParameter p1 = _AddParameter(doc, i, "DIM_HEIGHT", BuiltInParameterGroup.PG_GEOMETRY, false);
                    FamilyParameter p2 = _AddParameter(doc, i, "DIM_THICKNESS", BuiltInParameterGroup.PG_GEOMETRY, false);
                    FamilyParameter p3 = _AddParameter(doc, i, "DIM_WIDTH", BuiltInParameterGroup.PG_GEOMETRY, false);
                }
                if (i.Name == "FINISHES")
                {
                    FamilyParameter p1 = _AddParameter(doc, i, "FINISH_BLACK", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p2 = _AddParameter(doc, i, "FINISH_GALVANIZED", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p3 = _AddParameter(doc, i, "FINISH_OTHER", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p4 = _AddParameter(doc, i, "FINISH_PAINTED", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p5 = _AddParameter(doc, i, "FINISH_PRIMED", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p6 = _AddParameter(doc, i, "FINISH_STAINLESS", BuiltInParameterGroup.PG_MATERIALS, false);

                }

            }
            tran.Commit();
        }

        public void CIPCUSTOM(Document doc, c.Application app, Dictionary<string, string> dic)
        {
            FamilyManager familyManager = doc.FamilyManager;
            string origfile = app.SharedParametersFilename;
            string tempFile = @"C:\ProgramData\Autodesk\ApplicationPlugins\TVD\Shared_Params_2015_v01.txt";
            Transaction tran = new Transaction(doc, "add parameter");
            tran.Start();
            app.SharedParametersFilename = tempFile;
            DefinitionFile shareParameterfile = app.OpenSharedParameterFile();
            foreach (DefinitionGroup i in shareParameterfile.Groups)
            {
                if (i.Name == "IDENTITY")
                {
                    //group data
                    FamilyParameter p1 = _AddParameter(doc, i, "CONTROL_MARK", BuiltInParameterGroup.PG_DATA, false);
                    string vp1 = Getvalueparameter("CONTROL_MARK", dic);
                    familyManager.Set(p1, vp1);
                    FamilyParameter p2 = _AddParameter(doc, i, "IDENTITY_COMMENT", BuiltInParameterGroup.PG_DATA, false);
                    string vp2 = Getvalueparameter("IDENTITY_COMMENT", dic);
                    familyManager.Set(p2, vp2);
                    FamilyParameter p3 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    string vp3 = Getvalueparameter("IDENTITY_DESCRIPTION", dic);
                    familyManager.Set(p3, vp3);
                    FamilyParameter p4 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION_SHORT", BuiltInParameterGroup.PG_DATA, false);
                    string vp4 = Getvalueparameter("IDENTITY_DESCRIPTION_SHORT", dic);
                    familyManager.Set(p4, vp4);
                    FamilyParameter p5 = _AddParameter(doc, i, "MANUFACTURER_PLANT_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    string vp5 = Getvalueparameter("MANUFACTURER_PLANT_DESCRIPTION", dic);
                    familyManager.Set(p5, vp5);
                    FamilyParameter p6 = _AddParameter(doc, i, "MANUFACTURER_PLANT_ID", BuiltInParameterGroup.PG_DATA, false);
                    string vp6 = Getvalueparameter("MANUFACTURER_PLANT_ID", dic);
                    familyManager.Set(p6, vp6);
                    FamilyParameter p7 = _AddParameter(doc, i, "MANUFACTURE_COMPONENT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p8 = _AddParameter(doc, i, "SORTING_ORDER", BuiltInParameterGroup.PG_DATA, false);
                    familyManager.Set(p7, "CIP CUSTOM");
                    familyManager.Set(p8, int.Parse("301"));
                }
                if (i.Name == "DIMENSIONS_GENERAL")
                {
                    FamilyParameter p1 = _AddParameter(doc, i, "DIM_HEIGHT", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp1 = Getvalueparameter("DIM_HEIGHT", dic);
                    if (!string.IsNullOrEmpty(vp1))
                    {
                        familyManager.Set(p1, Convert.ToDouble(vp1));
                    }
                    FamilyParameter p2 = _AddParameter(doc, i, "DIM_THICKNESS", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp2 = Getvalueparameter("DIM_THICKNESS", dic);
                    if (!string.IsNullOrEmpty(vp2))
                    {
                        familyManager.Set(p2, Convert.ToDouble(vp2));
                    }
                    FamilyParameter p3 = _AddParameter(doc, i, "DIM_WIDTH", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp3 = Getvalueparameter("DIM_WIDTH", dic);
                    if (!string.IsNullOrEmpty(vp3))
                    {
                        familyManager.Set(p3, Convert.ToDouble(vp3));
                    }
                    FamilyParameter _p3 = _AddParameter(doc, i, "DBA_Length", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string _vp3 = Getvalueparameter("DBA_Length", dic);
                    if (!string.IsNullOrEmpty(_vp3))
                    {
                        familyManager.Set(_p3, Convert.ToDouble(_vp3));
                    }
                }
                //if (i.Name == "FINISHES")
                //{
                //    FamilyParameter p1 = _AddParameter(doc, i, "FINISH_BLACK", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p2 = _AddParameter(doc, i, "FINISH_GALVANIZED", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p3 = _AddParameter(doc, i, "FINISH_OTHER", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p4 = _AddParameter(doc, i, "FINISH_PAINTED", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p5 = _AddParameter(doc, i, "FINISH_PRIMED", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p6 = _AddParameter(doc, i, "FINISH_STAINLESS", BuiltInParameterGroup.PG_MATERIALS, false);
                //}

            }
            tran.Commit();

        }

        public void CIPSTANDARD(Document doc, c.Application app, Dictionary<string, string> dic)
        {
            FamilyManager familyManager = doc.FamilyManager;
            string origfile = app.SharedParametersFilename;
            string tempFile = @"C:\ProgramData\Autodesk\ApplicationPlugins\TVD\Shared_Params_2015_v01.txt";
            Transaction tran = new Transaction(doc, "add parameter");
            tran.Start();
            app.SharedParametersFilename = tempFile;
            DefinitionFile shareParameterfile = app.OpenSharedParameterFile();
            foreach (DefinitionGroup i in shareParameterfile.Groups)
            {
                if (i.Name == "IDENTITY")
                {
                    //group data
                    FamilyParameter p1 = _AddParameter(doc, i, "CONTROL_MARK", BuiltInParameterGroup.PG_DATA, false);
                    string vp1 = Getvalueparameter("CONTROL_MARK", dic);
                    familyManager.Set(p1, vp1);
                    FamilyParameter p2 = _AddParameter(doc, i, "IDENTITY_COMMENT", BuiltInParameterGroup.PG_DATA, false);
                    string vp2 = Getvalueparameter("IDENTITY_COMMENT", dic);
                    familyManager.Set(p2, vp2);
                    FamilyParameter p3 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    string vp3 = Getvalueparameter("IDENTITY_DESCRIPTION", dic);
                    familyManager.Set(p3, vp3);
                    FamilyParameter p4 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION_SHORT", BuiltInParameterGroup.PG_DATA, false);
                    string vp4 = Getvalueparameter("IDENTITY_DESCRIPTION_SHORT", dic);
                    familyManager.Set(p4, vp4);
                    FamilyParameter p5 = _AddParameter(doc, i, "MANUFACTURER_PLANT_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    string vp5 = Getvalueparameter("MANUFACTURER_PLANT_DESCRIPTION", dic);
                    familyManager.Set(p5, vp5);
                    FamilyParameter p6 = _AddParameter(doc, i, "MANUFACTURER_PLANT_ID", BuiltInParameterGroup.PG_DATA, false);
                    string vp6 = Getvalueparameter("MANUFACTURER_PLANT_ID", dic);
                    familyManager.Set(p6, vp6);
                    FamilyParameter p7 = _AddParameter(doc, i, "MANUFACTURE_COMPONENT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p8 = _AddParameter(doc, i, "SORTING_ORDER", BuiltInParameterGroup.PG_DATA, false);
                    familyManager.Set(p7, "CIP STANDARD");
                    familyManager.Set(p8, int.Parse("300"));
                }
                if (i.Name == "DIMENSIONS_GENERAL")
                {
                    FamilyParameter p1 = _AddParameter(doc, i, "DIM_HEIGHT", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp1 = Getvalueparameter("DIM_HEIGHT", dic);
                    if (!string.IsNullOrEmpty(vp1))
                    {
                        familyManager.Set(p1, Convert.ToDouble(vp1));
                    }
                    FamilyParameter p2 = _AddParameter(doc, i, "DIM_THICKNESS", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp2 = Getvalueparameter("DIM_THICKNESS", dic);
                    if (!string.IsNullOrEmpty(vp2))
                    {
                        familyManager.Set(p2, Convert.ToDouble(vp2));
                    }
                    FamilyParameter p3 = _AddParameter(doc, i, "DIM_WIDTH", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp3 = Getvalueparameter("DIM_WIDTH", dic);
                    if (!string.IsNullOrEmpty(vp3))
                    {
                        familyManager.Set(p3, Convert.ToDouble(vp3));
                    }
                    FamilyParameter _p3 = _AddParameter(doc, i, "DBA_Length", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string _vp3 = Getvalueparameter("DBA_Length", dic);
                    if (!string.IsNullOrEmpty(_vp3))
                    {
                        familyManager.Set(_p3, Convert.ToDouble(_vp3));
                    }
                }
                //if (i.Name == "FINISHES")
                //{
                //    FamilyParameter p1 = _AddParameter(doc, i, "FINISH_BLACK", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p2 = _AddParameter(doc, i, "FINISH_GALVANIZED", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p3 = _AddParameter(doc, i, "FINISH_OTHER", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p4 = _AddParameter(doc, i, "FINISH_PAINTED", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p5 = _AddParameter(doc, i, "FINISH_PRIMED", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p6 = _AddParameter(doc, i, "FINISH_STAINLESS", BuiltInParameterGroup.PG_MATERIALS, false);
                //}

            }
            tran.Commit();

        }
        public void ERCTIONSTANDARD_finish(Document doc, c.Application app, Dictionary<string, string> dic)
        {

            FamilyManager familyManager = doc.FamilyManager;
            string origfile = app.SharedParametersFilename;
            string tempFile = @"C:\ProgramData\Autodesk\ApplicationPlugins\TVD\Shared_Params_2015_v01.txt";
            Transaction tran = new Transaction(doc, "add parameter");
            tran.Start();
            app.SharedParametersFilename = tempFile;
            DefinitionFile shareParameterfile = app.OpenSharedParameterFile();
            FamilyParameter p10 = doc.FamilyManager.AddParameter("desc_b", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p11 = doc.FamilyManager.AddParameter("desc_g", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p12 = doc.FamilyManager.AddParameter("desc_o", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p13 = doc.FamilyManager.AddParameter("desc_pr", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p14 = doc.FamilyManager.AddParameter("desc_pt", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p15 = doc.FamilyManager.AddParameter("desc_s", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p16 = doc.FamilyManager.AddParameter("finish_check", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p17 = doc.FamilyManager.AddParameter("manufacturer_plant_description_b", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p18 = doc.FamilyManager.AddParameter("manufacturer_plant_description_g", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p19 = doc.FamilyManager.AddParameter("manufacturer_plant_description_o", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p20 = doc.FamilyManager.AddParameter("manufacturer_plant_description_pr", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p21 = doc.FamilyManager.AddParameter("manufacturer_plant_description_pt", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p22 = doc.FamilyManager.AddParameter("manufacturer_plant_description_s", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p23 = doc.FamilyManager.AddParameter("manufacturer_plant_id_b", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p24 = doc.FamilyManager.AddParameter("manufacturer_plant_id_g", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p25 = doc.FamilyManager.AddParameter("manufacturer_plant_id_o", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p26 = doc.FamilyManager.AddParameter("manufacturer_plant_id_pr", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p27 = doc.FamilyManager.AddParameter("manufacturer_plant_id_pt", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p28 = doc.FamilyManager.AddParameter("manufacturer_plant_id_s", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p29 = doc.FamilyManager.AddParameter("mark_b", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p30 = doc.FamilyManager.AddParameter("mark_g", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p31 = doc.FamilyManager.AddParameter("mark_o", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p32 = doc.FamilyManager.AddParameter("mark_pr", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p33 = doc.FamilyManager.AddParameter("mark_pt", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            FamilyParameter p34 = doc.FamilyManager.AddParameter("mark_s", BuiltInParameterGroup.INVALID, ParameterType.Text, false);
            foreach (DefinitionGroup i in shareParameterfile.Groups)
            {
                if (i.Name == "IDENTITY")
                {
                    //group data
                    FamilyParameter p1 = _AddParameter(doc, i, "CONTROL_MARK", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p2 = _AddParameter(doc, i, "IDENTITY_COMMENT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p3 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p4 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION_SHORT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p5 = _AddParameter(doc, i, "MANUFACTURER_PLANT_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p6 = _AddParameter(doc, i, "MANUFACTURER_PLANT_ID", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p7 = _AddParameter(doc, i, "MANUFACTURE_COMPONENT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p8 = _AddParameter(doc, i, "SORTING_ORDER", BuiltInParameterGroup.PG_DATA, false);
                    familyManager.SetFormula(p1, "if(FINISH_GALVANIZED, mark_g, (if(FINISH_PAINTED, mark_pt, (if(FINISH_PRIMED, mark_pr, (if(FINISH_OTHER, mark_o, (if(FINISH_STAINLESS, mark_s, mark_b)))))))))");
                    familyManager.SetFormula(p2, "if(FINISH_GALVANIZED, desc_g, (if(FINISH_PAINTED, desc_pt, (if(FINISH_PRIMED, desc_pr, (if(FINISH_OTHER, desc_o, (if(FINISH_STAINLESS, desc_s, desc_b)))))))))");
                    familyManager.SetFormula(p3, "if(FINISH_GALVANIZED, mark_g, (if(FINISH_PAINTED, mark_pt, (if(FINISH_PRIMED, mark_pr, (if(FINISH_OTHER, mark_o, (if(FINISH_STAINLESS, mark_s, mark_b)))))))))");
                    familyManager.SetFormula(p4, "if(FINISH_GALVANIZED, manufacturer_plant_description_g, (if(FINISH_PAINTED, manufacturer_plant_description_pt, (if(FINISH_PRIMED, manufacturer_plant_description_pr, (if(FINISH_OTHER, manufacturer_plant_description_o, (if(FINISH_STAINLESS, manufacturer_plant_description_s, manufacturer_plant_description_b)))))))))");
                    familyManager.SetFormula(p5, "if(FINISH_GALVANIZED, manufacturer_plant_id_g, (if(FINISH_PAINTED, manufacturer_plant_id_pt, (if(FINISH_PRIMED, manufacturer_plant_id_pr, (if(FINISH_OTHER, manufacturer_plant_id_o, (if(FINISH_STAINLESS, manufacturer_plant_id_s, manufacturer_plant_id_b)))))))))");
                    familyManager.Set(p7, "ERECTION STANDARD");
                    familyManager.Set(p8, int.Parse("200"));
                }
                if (i.Name == "DIMENSIONS_GENERAL")
                {
                    FamilyParameter p1 = _AddParameter(doc, i, "DIM_HEIGHT", BuiltInParameterGroup.PG_GEOMETRY, false);
                    FamilyParameter p2 = _AddParameter(doc, i, "DIM_THICKNESS", BuiltInParameterGroup.PG_GEOMETRY, false);
                    FamilyParameter p3 = _AddParameter(doc, i, "DIM_WIDTH", BuiltInParameterGroup.PG_GEOMETRY, false);
                }
                if (i.Name == "FINISHES")
                {
                    FamilyParameter p1 = _AddParameter(doc, i, "FINISH_BLACK", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p2 = _AddParameter(doc, i, "FINISH_GALVANIZED", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p3 = _AddParameter(doc, i, "FINISH_OTHER", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p4 = _AddParameter(doc, i, "FINISH_PAINTED", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p5 = _AddParameter(doc, i, "FINISH_PRIMED", BuiltInParameterGroup.PG_MATERIALS, false);
                    FamilyParameter p6 = _AddParameter(doc, i, "FINISH_STAINLESS", BuiltInParameterGroup.PG_MATERIALS, false);

                }

            }
            tran.Commit();
        }
        public void ERECTIONSTANDARD(Document doc, c.Application app, Dictionary<string, string> dic)
        {
            FamilyManager familyManager = doc.FamilyManager;
            string origfile = app.SharedParametersFilename;
            string tempFile = @"C:\ProgramData\Autodesk\ApplicationPlugins\TVD\Shared_Params_2015_v01.txt";
            Transaction tran = new Transaction(doc, "add parameter");
            tran.Start();
            app.SharedParametersFilename = tempFile;
            DefinitionFile shareParameterfile = app.OpenSharedParameterFile();
            foreach (DefinitionGroup i in shareParameterfile.Groups)
            {
                if (i.Name == "IDENTITY")
                {
                    //group data
                    FamilyParameter p1 = _AddParameter(doc, i, "CONTROL_MARK", BuiltInParameterGroup.PG_DATA, false);
                    string vp1 = Getvalueparameter("CONTROL_MARK", dic);
                    familyManager.Set(p1, vp1);
                    FamilyParameter p2 = _AddParameter(doc, i, "IDENTITY_COMMENT", BuiltInParameterGroup.PG_DATA, false);
                    string vp2 = Getvalueparameter("IDENTITY_COMMENT", dic);
                    familyManager.Set(p2, vp2);
                    FamilyParameter p3 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    string vp3 = Getvalueparameter("IDENTITY_DESCRIPTION", dic);
                    familyManager.Set(p3, vp3);
                    FamilyParameter p4 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION_SHORT", BuiltInParameterGroup.PG_DATA, false);
                    string vp4 = Getvalueparameter("IDENTITY_DESCRIPTION_SHORT", dic);
                    familyManager.Set(p4, vp4);
                    FamilyParameter p5 = _AddParameter(doc, i, "MANUFACTURER_PLANT_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    string vp5 = Getvalueparameter("MANUFACTURER_PLANT_DESCRIPTION", dic);
                    familyManager.Set(p5, vp5);
                    FamilyParameter p6 = _AddParameter(doc, i, "MANUFACTURER_PLANT_ID", BuiltInParameterGroup.PG_DATA, false);
                    string vp6 = Getvalueparameter("MANUFACTURER_PLANT_ID", dic);
                    familyManager.Set(p6, vp6);
                    FamilyParameter p7 = _AddParameter(doc, i, "MANUFACTURE_COMPONENT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p8 = _AddParameter(doc, i, "SORTING_ORDER", BuiltInParameterGroup.PG_DATA, false);
                    familyManager.Set(p7, "ERECTION STANDARD");
                    familyManager.Set(p8, int.Parse("200"));
                }
                if (i.Name == "DIMENSIONS_GENERAL")
                {
                    FamilyParameter p1 = _AddParameter(doc, i, "DIM_HEIGHT", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp1 = Getvalueparameter("DIM_HEIGHT", dic);
                    if (!string.IsNullOrEmpty(vp1))
                    {
                        familyManager.Set(p1, Convert.ToDouble(vp1));
                    }
                    FamilyParameter p2 = _AddParameter(doc, i, "DIM_THICKNESS", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp2 = Getvalueparameter("DIM_THICKNESS", dic);
                    if (!string.IsNullOrEmpty(vp2))
                    {
                        familyManager.Set(p2, Convert.ToDouble(vp2));
                    }
                    FamilyParameter p3 = _AddParameter(doc, i, "DIM_WIDTH", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp3 = Getvalueparameter("DIM_WIDTH", dic);
                    if (!string.IsNullOrEmpty(vp3))
                    {
                        familyManager.Set(p3, Convert.ToDouble(vp3));
                    }
                    FamilyParameter _p3 = _AddParameter(doc, i, "DBA_Length", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string _vp3 = Getvalueparameter("DBA_Length", dic);
                    if (!string.IsNullOrEmpty(_vp3))
                    {
                        familyManager.Set(_p3, Convert.ToDouble(_vp3));
                    }
                }
                //if (i.Name == "FINISHES")
                //{
                //    FamilyParameter p1 = _AddParameter(doc, i, "FINISH_BLACK", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p2 = _AddParameter(doc, i, "FINISH_GALVANIZED", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p3 = _AddParameter(doc, i, "FINISH_OTHER", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p4 = _AddParameter(doc, i, "FINISH_PAINTED", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p5 = _AddParameter(doc, i, "FINISH_PRIMED", BuiltInParameterGroup.PG_MATERIALS, false);
                //    FamilyParameter p6 = _AddParameter(doc, i, "FINISH_STAINLESS", BuiltInParameterGroup.PG_MATERIALS, false);
                //}

            }
            tran.Commit();

        }
        public void CONNECTION(Document doc, c.Application app)
        {
            FamilyManager familyManager = doc.FamilyManager;
            string origfile = app.SharedParametersFilename;
            string tempFile = @"C:\ProgramData\Autodesk\ApplicationPlugins\TVD\Shared_Params_2015_v01.txt";
            Transaction tran = new Transaction(doc, "add parameter");
            tran.Start();
            app.SharedParametersFilename = tempFile;
            DefinitionFile shareParameterfile = app.OpenSharedParameterFile();
            foreach (DefinitionGroup i in shareParameterfile.Groups)
            {
                if (i.Name == "IDENTITY")
                {
                    //group data
                    FamilyParameter p1 = _AddParameter(doc, i, "CONTROL_MARK", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p2 = _AddParameter(doc, i, "IDENTITY_COMMENT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p3 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p4 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION_SHORT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p5 = _AddParameter(doc, i, "MANUFACTURER_PLANT_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p6 = _AddParameter(doc, i, "MANUFACTURER_PLANT_ID", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p7 = _AddParameter(doc, i, "MANUFACTURE_COMPONENT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p8 = _AddParameter(doc, i, "SORTING_ORDER", BuiltInParameterGroup.PG_DATA, false);
                    familyManager.Set(p7, "CONNECTION");
                }

            }
            tran.Commit();

        }
        public void ERECTIONCUSTOM(Document doc, c.Application app, Dictionary<string, string> dic)
        {
            FamilyManager familyManager = doc.FamilyManager;
            string origfile = app.SharedParametersFilename;
            string tempFile = @"C:\ProgramData\Autodesk\ApplicationPlugins\TVD\Shared_Params_2015_v01.txt";
            Transaction tran = new Transaction(doc, "add parameter");
            tran.Start();
            app.SharedParametersFilename = tempFile;
            DefinitionFile shareParameterfile = app.OpenSharedParameterFile();
            foreach (DefinitionGroup i in shareParameterfile.Groups)
            {
                if (i.Name == "IDENTITY")
                {
                    //group data
                    FamilyParameter p1 = _AddParameter(doc, i, "CONTROL_MARK", BuiltInParameterGroup.PG_DATA, false);
                    string vp1 = Getvalueparameter("CONTROL_MARK", dic);
                    familyManager.Set(p1, vp1);
                    FamilyParameter p2 = _AddParameter(doc, i, "IDENTITY_COMMENT", BuiltInParameterGroup.PG_DATA, false);
                    string vp2 = Getvalueparameter("IDENTITY_COMMENT", dic);
                    familyManager.Set(p2, vp2);
                    FamilyParameter p3 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    string vp3 = Getvalueparameter("IDENTITY_DESCRIPTION", dic);
                    familyManager.Set(p3, vp3);
                    FamilyParameter p4 = _AddParameter(doc, i, "IDENTITY_DESCRIPTION_SHORT", BuiltInParameterGroup.PG_DATA, false);
                    string vp4 = Getvalueparameter("IDENTITY_DESCRIPTION_SHORT", dic);
                    familyManager.Set(p4, vp4);
                    FamilyParameter p5 = _AddParameter(doc, i, "MANUFACTURER_PLANT_DESCRIPTION", BuiltInParameterGroup.PG_DATA, false);
                    string vp5 = Getvalueparameter("MANUFACTURER_PLANT_DESCRIPTION", dic);
                    familyManager.Set(p5, vp5);
                    FamilyParameter p6 = _AddParameter(doc, i, "MANUFACTURER_PLANT_ID", BuiltInParameterGroup.PG_DATA, false);
                    string vp6 = Getvalueparameter("MANUFACTURER_PLANT_ID", dic);
                    familyManager.Set(p6, vp6);
                    FamilyParameter p7 = _AddParameter(doc, i, "MANUFACTURE_COMPONENT", BuiltInParameterGroup.PG_DATA, false);
                    FamilyParameter p8 = _AddParameter(doc, i, "SORTING_ORDER", BuiltInParameterGroup.PG_DATA, false);
                    familyManager.Set(p7, "ERECTION CUSTOM");
                    familyManager.Set(p8, int.Parse("201"));
                }
                if (i.Name == "DIMENSIONS_GENERAL")
                {
                    FamilyParameter p1 = _AddParameter(doc, i, "DIM_HEIGHT", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp1 = Getvalueparameter("DIM_HEIGHT", dic);
                    if (!string.IsNullOrEmpty(vp1))
                    {
                        familyManager.Set(p1, Convert.ToDouble(vp1));
                    }
                    FamilyParameter p2 = _AddParameter(doc, i, "DIM_THICKNESS", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp2 = Getvalueparameter("DIM_THICKNESS", dic);
                    if (!string.IsNullOrEmpty(vp2))
                    {
                        familyManager.Set(p2, Convert.ToDouble(vp2));
                    }
                    FamilyParameter p3 = _AddParameter(doc, i, "DIM_WIDTH", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string vp3 = Getvalueparameter("DIM_WIDTH", dic);
                    if (!string.IsNullOrEmpty(vp3))
                    {
                        familyManager.Set(p3, Convert.ToDouble(vp3));
                    }
                    FamilyParameter _p3 = _AddParameter(doc, i, "DBA_Length", BuiltInParameterGroup.PG_GEOMETRY, false);
                    string _vp3 = Getvalueparameter("DBA_Length", dic);
                    if (!string.IsNullOrEmpty(_vp3))
                    {
                        familyManager.Set(_p3, Convert.ToDouble(_vp3));
                    }
                }
            }
            tran.Commit();

        }
        public void AddPAnew(Document doc, c.Application app, Dictionary<string, string> dic)
        {
            FamilyManager familyManager = doc.FamilyManager;
            string origfile = app.SharedParametersFilename;
            string tempFile = @"C:\ProgramData\Autodesk\ApplicationPlugins\TVD\Shared_Params_2015_v01.txt";
            Transaction tran = new Transaction(doc, "add parameter");
            tran.Start();
            app.SharedParametersFilename = tempFile;
            DefinitionFile shareParameterfile = app.OpenSharedParameterFile();
            foreach (DefinitionGroup i in shareParameterfile.Groups)
            {
                if (i.Name == "MODEL_GENERAL")
                {
                    //group data
                    FamilyParameter p1 = _AddParameter(doc, i, "FAMILY_LEVEL", BuiltInParameterGroup.PG_TEXT, true);
                    string vp1 = Getvalueparameter("FAMILY_LEVEL", dic);
                    familyManager.Set(p1, vp1);
                    FamilyParameter p2 = _AddParameter(doc, i, "TYPE_DESCRIPTION", BuiltInParameterGroup.PG_TEXT, false);
                    string vp2 = Getvalueparameter("TYPE_DESCRIPTION", dic);
                    familyManager.Set(p2, vp2);
                }
            }
            tran.Commit();

        }
        public string Getvalueparameter(string Parameter, Dictionary<string,string> dic)
        {
            string value = string.Empty;
            foreach (var item in dic.Keys)
            {
                if(item.Equals(Parameter))
                {
                    value = dic[item];
                }
            }
            return value;
        }
        public void Changetypefamily(Document doc)
        {
            IList<Category> list = new List<Category>();
            Family f = doc.OwnerFamily;
            var f2 = f.GetType();
            BuiltInParameter _bip = BuiltInParameter.OMNICLASS_CODE;
            Category category = f.FamilyCategory;
            var t = f.StructuralFamilyNameKey;

        }
    }

}
