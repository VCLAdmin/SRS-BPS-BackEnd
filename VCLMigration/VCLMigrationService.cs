using System;
using BpsUnifiedModelLib;
using Newtonsoft.Json;
using VCLMigration.Model;
using System.Collections.Generic;

namespace VCLMigration
{
    public class VCLMigrationService
    {
        public string getV2UnifiedModel(string bpsUM_v1_string)
        {
            try
            {
                var hasInfillsAt = bpsUM_v1_string.IndexOf("Infills");
                BpsUnifiedModel_V1 bpsUM_v1 = new BpsUnifiedModel_V1();
                try
                {
                    try
                    {
                        bpsUM_v1 = JsonConvert.DeserializeObject<BpsUnifiedModel_V1>(bpsUM_v1_string);
                    }
                    catch (Exception ex)
                    {
                        //System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                        //dynamic item = serializer.Deserialize<object>(bpsUM_v1_string);
                        dynamic item = JsonConvert.DeserializeObject<object>(bpsUM_v1_string);
                        item["AnalysisResult"] = null;
                        //bpsUM_v1_string = serializer.Serialize(item);
                        bpsUM_v1_string = JsonConvert.SerializeObject(item);
                        bpsUM_v1 = JsonConvert.DeserializeObject<BpsUnifiedModel_V1>(bpsUM_v1_string);
                    }
                    if (bpsUM_v1.ModelInput == null || bpsUM_v1.UnifiedModelVersion == "V2" || hasInfillsAt != -1)
                    {
                        if (bpsUM_v1.UnifiedModelVersion == "V2")
                            return getV2UnifiedModelManageUpdate(bpsUM_v1_string);
                        else 
                            return bpsUM_v1_string;
                    }
                    else
                    {
                        BpsUnifiedModel bpsUM = new BpsUnifiedModel();
                        bpsUM.UnifiedModelVersion = "V2";
                        /* --- 1 User Setting --- */
                        #region UserSetting
                        bpsUM.UserSetting = new BpsUnifiedModelLib.UserSetting();
                        bpsUM.UserSetting.Language = bpsUM_v1.UserSetting.Language;
                        bpsUM.UserSetting.UserName = bpsUM_v1.UserSetting.UserName;
                        bpsUM.UserSetting.ApplicationType = "BPS";
                        #endregion
               
                        /* --- 2 Problem Setting --- */
                        #region ProblemSetting
                        if (bpsUM_v1.ProblemSetting != null)
                        {
                            bpsUM.ProblemSetting = new BpsUnifiedModelLib.ProblemSetting();

                            // for bps only
                            bpsUM.ProblemSetting.UserGuid = bpsUM_v1.ProblemSetting.UserGuid;
                            bpsUM.ProblemSetting.ProjectGuid = bpsUM_v1.ProblemSetting.ProjectGuid;
                            bpsUM.ProblemSetting.ProblemGuid = bpsUM_v1.ProblemSetting.ProblemGuid;
                            bpsUM.ProblemSetting.EnableAcoustic = bpsUM_v1.ProblemSetting.EnableAcoustic;
                            bpsUM.ProblemSetting.EnableStructural = bpsUM_v1.ProblemSetting.EnableStructural;
                            bpsUM.ProblemSetting.EnableThermal = bpsUM_v1.ProblemSetting.EnableThermal;
                            // for physics core
                            bpsUM.ProblemSetting.ProductType = bpsUM_v1.ProblemSetting.ProductType;
                            bpsUM.ProblemSetting.FacadeType = bpsUM_v1.ProblemSetting.FacadeType;
                            bpsUM.ProblemSetting.ProjectName = bpsUM_v1.ProblemSetting.ProjectName;
                            bpsUM.ProblemSetting.Location = bpsUM_v1.ProblemSetting.Location;
                            bpsUM.ProblemSetting.ConfigurationName = bpsUM_v1.ProblemSetting.ConfigurationName;
                            bpsUM.ProblemSetting.UserNotes = bpsUM_v1.ProblemSetting.UserNotes;
                            // for SRS
                            //public string Client;
                            //public string ProjectNumber;
                            //public string LastModifiedDate;
                            //public List<string> DrawingNames;
                        }
                        #endregion

                        /* --- 3 ModelInput --- */
                        #region ModelInput
                        bpsUM.ModelInput = new BpsUnifiedModelLib.ModelInput();
                        #region FrameSystem
                        if (bpsUM_v1.ModelInput.FrameSystem != null)
                        {
                            bpsUM.ModelInput.FrameSystem = new BpsUnifiedModelLib.FrameSystem();
                            bpsUM.ModelInput.FrameSystem.SystemType = bpsUM_v1.ModelInput.FrameSystem.SystemType;
                            bpsUM.ModelInput.FrameSystem.InsulationZone = bpsUM_v1.ModelInput.FrameSystem.InsulationZone;

                            bpsUM.ModelInput.FrameSystem.UvalueType = bpsUM_v1.ModelInput.FrameSystem.UvalueType;
                            bpsUM.ModelInput.FrameSystem.InsulationType = bpsUM_v1.ModelInput.FrameSystem.InsulationType;
                            bpsUM.ModelInput.FrameSystem.InsulatingBarDataNote = bpsUM_v1.ModelInput.FrameSystem.InsulatingBarDataNote;
                            bpsUM.ModelInput.FrameSystem.InsulationMaterial = bpsUM_v1.ModelInput.FrameSystem.InsulationMaterial;
                            bpsUM.ModelInput.FrameSystem.Alloys = bpsUM_v1.ModelInput.FrameSystem.Alloys;
                            bpsUM.ModelInput.FrameSystem.xNumberOfPanels = bpsUM_v1.ModelInput.FrameSystem.xNumberOfPanels;
                            bpsUM.ModelInput.FrameSystem.yNumberOfPanels = bpsUM_v1.ModelInput.FrameSystem.yNumberOfPanels;

                            //// for facade only
                            //public double MajorMullionTopRecess;            //UnifiedInputVersion2.0. For SRS.
                            //public double MajorMullionBottomRecess;            //UnifiedInputVersion2.0. For SRS.

                            //// for UDC only
                            bpsUM.ModelInput.FrameSystem.VerticalJointWidth = bpsUM_v1.ModelInput.FrameSystem.VerticalJointWidth;
                            bpsUM.ModelInput.FrameSystem.HorizontalJointWidth = bpsUM_v1.ModelInput.FrameSystem.HorizontalJointWidth;

                            //// for SRS
                            //public string AluminumFinish;
                            //public string AluminumColor;
                        }
                        #endregion
                        #region Geometry
                        bpsUM.ModelInput.Geometry = new BpsUnifiedModelLib.Geometry();
                        #region Points
                        if (bpsUM_v1.ModelInput.Geometry.Points != null)
                        {
                            bpsUM.ModelInput.Geometry.Points = new List<BpsUnifiedModelLib.Point>();
                            foreach (var item in bpsUM_v1.ModelInput.Geometry.Points)
                            {
                                BpsUnifiedModelLib.Point point = new BpsUnifiedModelLib.Point();
                                point.PointID = item.PointID;
                                point.X = item.X;
                                point.Y = item.Y;
                                bpsUM.ModelInput.Geometry.Points.Add(point);
                            }
                        }
                        #endregion
                        #region Members
                        if (bpsUM_v1.ModelInput.Geometry.Members != null)
                        {
                            bpsUM.ModelInput.Geometry.Members = new List<BpsUnifiedModelLib.Member>();
                            foreach (var item in bpsUM_v1.ModelInput.Geometry.Members)
                            {
                                BpsUnifiedModelLib.Member member = new BpsUnifiedModelLib.Member();
                                member.MemberID = item.MemberID;
                                member.PointA = item.PointA;
                                member.PointB = item.PointB;
                                member.SectionID = item.SectionID;
                                member.MemberType = item.MemberType;
                                member.Length_cm = item.Length_cm;
                                member.TributaryArea = item.TributaryArea;
                                member.TributaryAreaFactor = item.TributaryAreaFactor;
                                member.Cp = item.Cp;
                                bpsUM.ModelInput.Geometry.Members.Add(member);
                            }
                        }
                        #endregion
                        #region Infills / GlassList
                        if (bpsUM_v1.ModelInput.Geometry.GlassList != null)
                        {
                            bpsUM.ModelInput.Geometry.Infills = new List<Infill>();
                            foreach (var item in bpsUM_v1.ModelInput.Geometry.GlassList)
                            {
                                BpsUnifiedModelLib.Infill infill = new BpsUnifiedModelLib.Infill();
                                infill.InfillID = item.GlassID;//"GlassID": 1,
                                infill.BoundingMembers = item.BoundingMembers;//"BoundingMembers": [1, 8, 2, 5],
                                infill.GlazingSystemID = item.GlazingSystemID;//"GlazingSystemID": 1,
                                infill.PanelSystemID = item.PanelSystemID;//"PanelSystemID": -1,
                                infill.BlockDistance = item.BlockDistance;//"BlockDistance": 100,

                                if (item.InsertWindowSystem != null)
                                {
                                    infill.OperabilitySystemID = item.OperabilitySystemID + 1;//"OperabilitySystemID": 0,
                                    infill.InsertOuterFrameDepth = item.InsertOuterFrameDepth;//"InsertOuterFrameDepth": 0,
                                    infill.InsertWindowSystem = item.InsertWindowSystem; //"InsertWindowSystem": null
                                    infill.InsertWindowSystemType = item.InsertWindowSystemType; //"InsertWindowSystemType": null
                                }
                                else
                                    infill.OperabilitySystemID = -1;

                                bpsUM.ModelInput.Geometry.Infills.Add(infill);
                            }
                        }
                        #region Custom Glass
                        if (bpsUM_v1.ModelInput.Geometry.CustomGlass != null)
                        {
                            bpsUM.ModelInput.Geometry.CustomGlass = new List<BpsUnifiedModelLib.CustomGlass>();
                            foreach (var item in bpsUM_v1.ModelInput.Geometry.CustomGlass)
                            {
                                BpsUnifiedModelLib.CustomGlass customGlass = new BpsUnifiedModelLib.CustomGlass();

                                customGlass.customGlassID = item.customGlassID;
                                customGlass.selectedType = item.selectedType;
                                customGlass.name = item.name;
                                customGlass.element_xx_1 = item.element_xx_1;
                                customGlass.element_type_1 = item.element_type_1;
                                customGlass.element_size_1 = item.element_size_1;
                                customGlass.element_interlayer_1 = item.element_interlayer_1;
                                customGlass.element_ins_type_1 = item.element_ins_type_1;
                                customGlass.element_ins_size_1 = item.element_ins_size_1;
                                customGlass.element_xx_2 = item.element_xx_2;
                                customGlass.element_type_2 = item.element_type_2;
                                customGlass.element_size_2 = item.element_size_2;
                                customGlass.element_interlayer_2 = item.element_interlayer_2;
                                customGlass.element_ins_type_2 = item.element_ins_type_2;
                                customGlass.element_ins_size_2 = item.element_ins_size_2;
                                customGlass.element_xx_3 = item.element_xx_3;
                                customGlass.element_type_3 = item.element_type_3;
                                customGlass.element_size_3 = item.element_size_3;
                                customGlass.element_interlayer_3 = item.element_interlayer_3;
                                customGlass.uValue = item.uValue;
                                customGlass.glassrw = item.glassrw;

                                bpsUM.ModelInput.Geometry.CustomGlass.Add(customGlass);
                            }
                        }
                        #endregion
                        #endregion
                        #region OperabilitySystems
                        if (bpsUM_v1.ModelInput.Geometry.GlassList != null)
                        {
                            bpsUM.ModelInput.Geometry.OperabilitySystems = new List<OperabilitySystem>();
                            foreach (var item in bpsUM_v1.ModelInput.Geometry.GlassList)
                            {
                                if (item.InsertedWindowType != null)
                                {
                                    BpsUnifiedModelLib.OperabilitySystem operabilitySystems = new BpsUnifiedModelLib.OperabilitySystem();
                                    operabilitySystems.OperabilitySystemID = item.OperabilitySystemID + 1;//"OperabilitySystemID": 0,
                                    operabilitySystems.VentArticleName = item.VentArticleName;//"VentArticleName": "-1",
                                    operabilitySystems.VentInsideW = item.VentInsideW;//"VentInsideW": 0,
                                    operabilitySystems.VentOutsideW = item.VentOutsideW;//"VentOutsideW": 0,
                                    operabilitySystems.VentDistBetweenIsoBars = item.VentDistBetweenIsoBars;//"VentDistBetweenIsoBars": 0,
                                    operabilitySystems.JunctionType = item.JunctionType;//"JunctionType": 0,
                                    operabilitySystems.InsertedWindowType = item.InsertedWindowType;//"InsertedWindowType": null,
                                    operabilitySystems.InsertOuterFrameArticleName = item.InsertOuterFrameArticleName;//"InsertOuterFrameArticleName": null,
                                    operabilitySystems.InsertOuterFrameInsideW = item.InsertOuterFrameInsideW;//"InsertOuterFrameInsideW": 0,
                                    operabilitySystems.InsertOuterFrameOutsideW = item.InsertOuterFrameOutsideW;//"InsertOuterFrameOutsideW": 0,
                                    operabilitySystems.InsertOuterFrameDistBetweenIsoBars = item.InsertOuterFrameDistBetweenIsoBars;//"InsertOuterFrameDistBetweenIsoBars": 0,
                                    operabilitySystems.InsertUvalueType = item.InsertUvalueType;//"InsertUvalueType": "AGF",
                                    operabilitySystems.InsertInsulationType = item.InsertInsulationType;//"InsertInsulationType": "PA",
                                                                                                        //"InsertInsulationTypeName": "Polyamide Coated After",
                                    operabilitySystems.VentOpeningDirection = item.VentOpeningDirection;//"VentOpeningDirection": null,


                                    if(item.VentOperableType == "Tilt-Turn-Right")
                                        operabilitySystems.VentOperableType = "Turn-Tilt-Right";
                                    else if (item.VentOperableType == "Tilt-Turn-Left")
                                        operabilitySystems.VentOperableType = "Turn-Tilt-Left";                                
                                    else 
                                        operabilitySystems.VentOperableType = item.VentOperableType;//"VentOperableType": null,
                                    //"centerX": 0,
                                    //"centerY": 0,
                                    bpsUM.ModelInput.Geometry.OperabilitySystems.Add(operabilitySystems);
                                }
                            }
                        }
                        #endregion
                        #region SlabAnchors
                        if (bpsUM_v1.ModelInput.Geometry.SlabAnchors != null)
                        {
                            bpsUM.ModelInput.Geometry.SlabAnchors = new List<BpsUnifiedModelLib.SlabAnchor>();
                            foreach (var item in bpsUM_v1.ModelInput.Geometry.SlabAnchors)
                            {
                                BpsUnifiedModelLib.SlabAnchor slabAnchor = new BpsUnifiedModelLib.SlabAnchor();
                                slabAnchor.SlabAnchorID = item.SlabAnchorID;
                                slabAnchor.MemberID = item.MemberID;
                                slabAnchor.AnchorType = item.AnchorType;
                                slabAnchor.Y = item.Y;
                                bpsUM.ModelInput.Geometry.SlabAnchors.Add(slabAnchor);
                            }
                        }
                        #endregion
                        #region Reinforcements
                        if (bpsUM_v1.ModelInput.Geometry.Reinforcements != null)
                        {
                            bpsUM.ModelInput.Geometry.Reinforcements = new List<BpsUnifiedModelLib.Reinforcement>();
                            foreach (var item in bpsUM_v1.ModelInput.Geometry.Reinforcements)
                            {
                                BpsUnifiedModelLib.Reinforcement reinforcement = new BpsUnifiedModelLib.Reinforcement();
                                reinforcement.ReinforcementID = item.ReinforcementID;
                                reinforcement.MemberID = item.MemberID;
                                reinforcement.SectionID = item.SectionID;
                                bpsUM.ModelInput.Geometry.Reinforcements.Add(reinforcement);
                            }
                        }
                        #endregion
                        #region SpliceJoints
                        if (bpsUM_v1.ModelInput.Geometry.SpliceJoints != null)
                        {
                            bpsUM.ModelInput.Geometry.SpliceJoints = new List<BpsUnifiedModelLib.SpliceJoint>();
                            foreach (var item in bpsUM_v1.ModelInput.Geometry.SpliceJoints)
                            {
                                BpsUnifiedModelLib.SpliceJoint spliceJoint = new BpsUnifiedModelLib.SpliceJoint();
                                spliceJoint.SpliceJointID = item.SpliceJointID;
                                spliceJoint.MemberID = item.MemberID;
                                spliceJoint.JointType = item.JointType;
                                spliceJoint.Y = item.Y;
                                bpsUM.ModelInput.Geometry.SpliceJoints.Add(spliceJoint);
                            }
                        }
                        #endregion
                        #region GlazingSystems
                        if (bpsUM_v1.ModelInput.Geometry.GlazingSystems != null)
                        {

                            bpsUM.ModelInput.Geometry.GlazingSystems = new List<BpsUnifiedModelLib.GlazingSystem>();
                            foreach (var item in bpsUM_v1.ModelInput.Geometry.GlazingSystems)
                            {
                                BpsUnifiedModelLib.GlazingSystem glazingSystem = new BpsUnifiedModelLib.GlazingSystem();
                                glazingSystem.GlazingSystemID = item.GlazingSystemID;//"GlazingSystemID": 1,
                                glazingSystem.Rw = item.Rw;//"Rw": 3,
                                glazingSystem.UValue = item.UValue;//"UValue": 1,
                                glazingSystem.SpacerType = item.SpacerType;//"SpacerType": 1,
                                glazingSystem.Description = item.Description;//"Description": "4/12/4 (20 mm)",
                                #region Plates
                                if (item.Plates != null)
                                {
                                    glazingSystem.Plates = new List<BpsUnifiedModelLib.Plate>();
                                    foreach (var subItem in item.Plates)
                                    {
                                        BpsUnifiedModelLib.Plate plate = new BpsUnifiedModelLib.Plate();
                                        plate.Material = subItem.Material;//"Material": "Glass",
                                        plate.H = subItem.H == null ? 0 : (double)subItem.H;//"H": 4,
                                        plate.InterMaterial = subItem.InterMaterial;//"InterMaterial": null,
                                        plate.InterH = subItem.InterH == null ? 0 : (double)subItem.InterH;//"InterH": 0
                                        glazingSystem.Plates.Add(plate);
                                    }
                                }
                                #endregion
                                #region Cavities
                                if (item.Cavities != null)
                                {
                                    glazingSystem.Cavities = new List<BpsUnifiedModelLib.Cavity>();
                                    foreach (var subItem in item.Cavities)
                                    {
                                        BpsUnifiedModelLib.Cavity cavity = new BpsUnifiedModelLib.Cavity();
                                        cavity.CavityType = subItem.CavityType;//"CavityType": "Air",
                                        cavity.Lz = subItem.Lz;//"Lz": 12
                                        glazingSystem.Cavities.Add(cavity);
                                    }
                                }
                                #endregion
                                glazingSystem.Category = item.Category;//"Category": "custom-double",
                                glazingSystem.PSIValue = item.PSIValue;//"PSIValue": 0
                                bpsUM.ModelInput.Geometry.GlazingSystems.Add(glazingSystem);
                            }
                        }
                        #endregion
                        #region PanelSystems
                        bpsUM.ModelInput.Geometry.PanelSystems = new List<BpsUnifiedModelLib.PanelSystem>();
                        if (bpsUM_v1.ModelInput.Geometry.PanelSystems != null)
                            foreach (var item in bpsUM_v1.ModelInput.Geometry.PanelSystems)
                            {
                                BpsUnifiedModelLib.PanelSystem panelSystem = new BpsUnifiedModelLib.PanelSystem();
                                panelSystem.PanelSystemID = item.PanelSystemID;
                                panelSystem.PanelID = item.PanelID;
                                panelSystem.Rw = item.Rw;
                                panelSystem.UValue = item.UValue;
                                panelSystem.PanelType = item.PanelType;
                                panelSystem.Psi = item.Psi;
                                panelSystem.Description = item.Description;
                                panelSystem.Thickness = item.Thickness;
                                #region Plates
                                if (item.Plates != null)
                                {
                                    panelSystem.Plates = new List<BpsUnifiedModelLib.Plate>();
                                    foreach (var subItem in item.Plates)
                                    {
                                        BpsUnifiedModelLib.Plate plate = new BpsUnifiedModelLib.Plate();
                                        plate.Material = subItem.Material;
                                        plate.H = subItem.H == null ? 0 : (double)subItem.H;
                                        plate.InterMaterial = subItem.InterMaterial;
                                        plate.InterH = subItem.InterH == null ? 0 : (double)subItem.InterH;
                                        panelSystem.Plates.Add(plate);
                                    }
                                }
                                #endregion
                                #region Cavities
                                if (item.Cavities != null)
                                {
                                    panelSystem.Cavities = new List<BpsUnifiedModelLib.Cavity>();
                                    foreach (var subItem in item.Cavities)
                                    {
                                        BpsUnifiedModelLib.Cavity cavity = new BpsUnifiedModelLib.Cavity();
                                        cavity.CavityType = subItem.CavityType;
                                        cavity.Lz = subItem.Lz;
                                        panelSystem.Cavities.Add(cavity);
                                    }
                                }
                                #endregion
                                bpsUM.ModelInput.Geometry.PanelSystems.Add(panelSystem);
                            }
                        #endregion
                        #region Sections
                        bpsUM.ModelInput.Geometry.Sections = new List<BpsUnifiedModelLib.Section>();
                        if (bpsUM_v1.ModelInput.Geometry.Sections != null)
                            foreach (var item in bpsUM_v1.ModelInput.Geometry.Sections)
                            {
                                BpsUnifiedModelLib.Section section = new BpsUnifiedModelLib.Section();
                                section.SectionID = item.SectionID;
                                section.SectionType = item.SectionType;
                                section.ArticleName = item.ArticleName;
                                section.isCustomProfile = item.isCustomProfile;
                                section.InsideW = item.InsideW;
                                section.OutsideW = item.OutsideW;
                                section.LeftRebate = item.LeftRebate;
                                section.RightRebate = item.RightRebate;
                                section.DistBetweenIsoBars = item.DistBetweenIsoBars;
                                section.d = item.d;
                                section.Weight = item.Weight;
                                section.Ao = item.Ao;
                                section.Au = item.Au;
                                section.Io = item.Io;
                                section.Iu = item.Iu;
                                section.Ioyy = item.Ioyy;
                                section.Iuyy = item.Iuyy;
                                section.Zoo = item.Zoo;
                                section.Zou = item.Zou;
                                section.Zol = item.Zol;
                                section.Zor = item.Zor;
                                section.Zuo = item.Zuo;
                                section.Zuu = item.Zuu;
                                section.Zul = item.Zul;
                                section.Zur = item.Zur;
                                section.RSn20 = item.RSn20;
                                section.RSp80 = item.RSp80;
                                section.RTn20 = item.RTn20;
                                section.RTp80 = item.RTp80;
                                section.Cn20 = item.Cn20;
                                section.Cp20 = item.Cp20;
                                section.Cp80 = item.Cp80;
                                section.beta = item.beta;
                                section.A2 = item.A2;
                                section.E = item.E;
                                section.alpha = item.alpha;
                                //public double Depth;
                                bpsUM.ModelInput.Geometry.Sections.Add(section);
                            }
                        #endregion
                        #region FacadeSections
                        bpsUM.ModelInput.Geometry.FacadeSections = new List<BpsUnifiedModelLib.FacadeSection>();
                        if (bpsUM_v1.ModelInput.Geometry.FacadeSections != null)
                            foreach (var item in bpsUM_v1.ModelInput.Geometry.FacadeSections)
                            {
                                BpsUnifiedModelLib.FacadeSection facadeSection = new BpsUnifiedModelLib.FacadeSection();
                                facadeSection.SectionID = item.SectionID;
                                facadeSection.SectionType = item.SectionType;
                                facadeSection.ArticleName = oldToNewArticle(item.ArticleName);
                                if(item.Depth == 0)
                                    facadeSection.Depth = getArticleDepth(item.ArticleName);
                                else 
                                    facadeSection.Depth = item.Depth;
                                //public bool isCustomProfile;   // is custom profile
                                facadeSection.OutsideW = item.OutsideW;
                                facadeSection.BTDepth = item.BTDepth;
                                facadeSection.Width = item.Width;
                                facadeSection.Zo = item.Zo;
                                facadeSection.Zu = item.Zu;
                                facadeSection.Zl = item.Zl;
                                facadeSection.Zr = item.Zr;
                                facadeSection.A = item.A;
                                facadeSection.Material = item.Material;
                                facadeSection.beta = item.beta;
                                facadeSection.Weight = item.Weight;
                                facadeSection.Iyy = item.Iyy;
                                facadeSection.Izz = item.Izz;
                                facadeSection.Wyy = item.Wyy;
                                facadeSection.Wzz = item.Wzz;
                                facadeSection.Asy = item.Asy;
                                facadeSection.Asz = item.Asz;
                                facadeSection.J = item.J;
                                facadeSection.E = item.E;
                                facadeSection.G = item.G;
                                facadeSection.EA = item.EA;
                                facadeSection.GAsy = item.GAsy;
                                facadeSection.GAsz = item.GAsz;
                                facadeSection.EIy = item.EIy;
                                facadeSection.EIz = item.EIz;
                                facadeSection.GJ = item.GJ;
                                facadeSection.Ys = item.Ys;
                                facadeSection.Zs = item.Zs;
                                facadeSection.Ry = item.Ry;
                                facadeSection.Rz = item.Rz;
                                facadeSection.Wyp = item.Wyp;
                                facadeSection.Wyn = item.Wyn;
                                facadeSection.Wzp = item.Wzp;
                                facadeSection.Wzn = item.Wzn;
                                facadeSection.Cw = item.Cw;
                                facadeSection.Beta_torsion = item.Beta_torsion;
                                facadeSection.Zy = item.Zy;
                                facadeSection.Zz = item.Zz;
                                bpsUM.ModelInput.Geometry.FacadeSections.Add(facadeSection);
                            }
                        #endregion
                        #endregion
                        #region Acoustic
                        if (bpsUM_v1.ModelInput.Acoustic != null)
                        {
                            bpsUM.ModelInput.Acoustic = new BpsUnifiedModelLib.Acoustic();
                            bpsUM.ModelInput.Acoustic.WallType = bpsUM_v1.ModelInput.Acoustic.WallType;
                            bpsUM.ModelInput.Acoustic.Height = bpsUM_v1.ModelInput.Acoustic.Height;
                            bpsUM.ModelInput.Acoustic.Width = bpsUM_v1.ModelInput.Acoustic.Width;
                            bpsUM.ModelInput.Acoustic.RoomArea = bpsUM_v1.ModelInput.Acoustic.RoomArea;
                        }
                        #endregion
                        #region Structural
                        if (bpsUM_v1.ModelInput.Structural != null)
                        {
                            bpsUM.ModelInput.Structural = new BpsUnifiedModelLib.Structural();
                            bpsUM.ModelInput.Structural.DispIndexType = bpsUM_v1.ModelInput.Structural.DispIndexType;
                            bpsUM.ModelInput.Structural.DispHorizontalIndex = bpsUM_v1.ModelInput.Structural.DispHorizontalIndex;
                            bpsUM.ModelInput.Structural.DispVerticalIndex = bpsUM_v1.ModelInput.Structural.DispVerticalIndex;
                            bpsUM.ModelInput.Structural.WindLoadInputType = bpsUM_v1.ModelInput.Structural.WindLoadInputType;
                            #region dinWindLoadInput
                            if (bpsUM_v1.ModelInput.Structural.dinWindLoadInput != null)
                            {
                                bpsUM.ModelInput.Structural.dinWindLoadInput = new BpsUnifiedModelLib.DinWindLoadInput();
                                bpsUM.ModelInput.Structural.dinWindLoadInput.WindZone = bpsUM_v1.ModelInput.Structural.dinWindLoadInput.WindZone;
                                bpsUM.ModelInput.Structural.dinWindLoadInput.TerrainCategory = bpsUM_v1.ModelInput.Structural.dinWindLoadInput.TerrainCategory;
                                bpsUM.ModelInput.Structural.dinWindLoadInput.L0 = bpsUM_v1.ModelInput.Structural.dinWindLoadInput.L0;
                                bpsUM.ModelInput.Structural.dinWindLoadInput.B0 = bpsUM_v1.ModelInput.Structural.dinWindLoadInput.B0;
                                bpsUM.ModelInput.Structural.dinWindLoadInput.h = bpsUM_v1.ModelInput.Structural.dinWindLoadInput.h;
                                bpsUM.ModelInput.Structural.dinWindLoadInput.ElvW = bpsUM_v1.ModelInput.Structural.dinWindLoadInput.ElvW;
                                bpsUM.ModelInput.Structural.dinWindLoadInput.WindowZone = bpsUM_v1.ModelInput.Structural.dinWindLoadInput.WindowZone;
                                bpsUM.ModelInput.Structural.dinWindLoadInput.IncludeCpi = bpsUM_v1.ModelInput.Structural.dinWindLoadInput.IncludeCpi;
                                
                                //public double pCpi;                 // user positive Cpi, added 2021.09.03
                                //public double nCpi;                 // user negative Cpi, added 2021.09.03
                                ////for internal use
                                //public bool RequestDescription;
                                ////for internal use angular
                                //public long PostCodeValue;
                            }
                            #endregion
                            bpsUM.ModelInput.Structural.WindLoad = bpsUM_v1.ModelInput.Structural.WindLoad;
                            bpsUM.ModelInput.Structural.Cpp = bpsUM_v1.ModelInput.Structural.Cpp;
                            bpsUM.ModelInput.Structural.Cpn = bpsUM_v1.ModelInput.Structural.Cpn;
                            bpsUM.ModelInput.Structural.HorizontalLiveLoad = bpsUM_v1.ModelInput.Structural.HorizontalLiveLoad;
                            bpsUM.ModelInput.Structural.HorizontalLiveLoadHeight = bpsUM_v1.ModelInput.Structural.HorizontalLiveLoadHeight;
                            #region LoadFactor
                            if (bpsUM_v1.ModelInput.Structural.LoadFactor != null)
                            {
                                bpsUM.ModelInput.Structural.LoadFactor = new BpsUnifiedModelLib.LoadFactor();
                                bpsUM.ModelInput.Structural.LoadFactor.DeadLoadFactor = bpsUM_v1.ModelInput.Structural.LoadFactor.DeadLoadFactor;
                                bpsUM.ModelInput.Structural.LoadFactor.WindLoadFactor = bpsUM_v1.ModelInput.Structural.LoadFactor.WindLoadFactor;
                                bpsUM.ModelInput.Structural.LoadFactor.HorizontalLiveLoadFactor = bpsUM_v1.ModelInput.Structural.LoadFactor.HorizontalLiveLoadFactor;
                                bpsUM.ModelInput.Structural.LoadFactor.TemperatureLoadFactor = bpsUM_v1.ModelInput.Structural.LoadFactor.TemperatureLoadFactor;
                            }
                            #endregion
                            #region SeasonFactor
                            if (bpsUM_v1.ModelInput.Structural.SeasonFactor != null)
                            {
                                bpsUM.ModelInput.Structural.SeasonFactor = new BpsUnifiedModelLib.SeasonFactor();
                                bpsUM.ModelInput.Structural.SeasonFactor.SummerFactor = bpsUM_v1.ModelInput.Structural.SeasonFactor.SummerFactor;
                                bpsUM.ModelInput.Structural.SeasonFactor.WinterFactor = bpsUM_v1.ModelInput.Structural.SeasonFactor.WinterFactor;
                            }
                            #endregion
                            #region TemperatureChange
                            if (bpsUM_v1.ModelInput.Structural.TemperatureChange != null)
                            {
                                bpsUM.ModelInput.Structural.TemperatureChange = new BpsUnifiedModelLib.TempChange();
                                bpsUM.ModelInput.Structural.TemperatureChange.Summer = bpsUM_v1.ModelInput.Structural.TemperatureChange.Summer;
                                bpsUM.ModelInput.Structural.TemperatureChange.Winter = bpsUM_v1.ModelInput.Structural.TemperatureChange.Winter;
                            }


                            //// for internal use
                            //public bool ShowBoundaryCondition;
                            //public bool ShowWindPressure;
                            //public string PositiveWindPressure;
                            //public string NegativeWindPressure;
                            #endregion
                        }
                        #endregion
                        #region Thermal
                        if (bpsUM_v1.ModelInput.Thermal != null)
                        {
                            bpsUM.ModelInput.Thermal = new BpsUnifiedModelLib.Thermal();
                            bpsUM.ModelInput.Thermal.RelativeHumidity = bpsUM_v1.ModelInput.Thermal.RelativeHumidity;
                            bpsUM.ModelInput.Thermal.InsulationZone = bpsUM_v1.ModelInput.Thermal.InsulationZone;
                        }
                        #endregion
                        #endregion

                        /* --- 4 AnalysisResult --- */
                        #region AnalysisResult
                        if (bpsUM_v1.AnalysisResult != null)
                        {

                            bpsUM.AnalysisResult = new BpsUnifiedModelLib.AnalysisResult();
                            /* --- 1 Acoustic Result --- */
                            #region AcousticResult
                            if (bpsUM_v1.AnalysisResult.AcousticResult != null)
                            {
                                bpsUM.AnalysisResult.AcousticResult = new BpsUnifiedModelLib.AcousticResult();
                                #region AcousticUIOutput
                                bpsUM.AnalysisResult.AcousticResult.AcousticUIOutput = new BpsUnifiedModelLib.AcousticOutput();
                                if (bpsUM_v1.AnalysisResult.AcousticResult.AcousticUIOutput.classification != null)
                                {
                                    bpsUM.AnalysisResult.AcousticResult.AcousticUIOutput.classification = new BpsUnifiedModelLib.Classification();
                                    bpsUM.AnalysisResult.AcousticResult.AcousticUIOutput.classification.STC = bpsUM_v1.AnalysisResult.AcousticResult.AcousticUIOutput.classification.STC;
                                    bpsUM.AnalysisResult.AcousticResult.AcousticUIOutput.classification.OITC = bpsUM_v1.AnalysisResult.AcousticResult.AcousticUIOutput.classification.OITC;
                                    bpsUM.AnalysisResult.AcousticResult.AcousticUIOutput.classification.Rw = bpsUM_v1.AnalysisResult.AcousticResult.AcousticUIOutput.classification.Rw;
                                    bpsUM.AnalysisResult.AcousticResult.AcousticUIOutput.classification.C = bpsUM_v1.AnalysisResult.AcousticResult.AcousticUIOutput.classification.C;
                                    bpsUM.AnalysisResult.AcousticResult.AcousticUIOutput.classification.Ctr = bpsUM_v1.AnalysisResult.AcousticResult.AcousticUIOutput.classification.Ctr;
                                    bpsUM.AnalysisResult.AcousticResult.AcousticUIOutput.classification.NC = bpsUM_v1.AnalysisResult.AcousticResult.AcousticUIOutput.classification.NC;
                                    bpsUM.AnalysisResult.AcousticResult.AcousticUIOutput.classification.Deficiencies = bpsUM_v1.AnalysisResult.AcousticResult.AcousticUIOutput.classification.Deficiencies;
                                }
                                var openDistribution = new BpsUnifiedModelLib.LossDistributionPoint[bpsUM_v1.AnalysisResult.AcousticResult.AcousticUIOutput.LossDistributions.Count];
                                bpsUM.AnalysisResult.AcousticResult.AcousticUIOutput.LossDistributions = new List<BpsUnifiedModelLib.LossDistributionPoint[]>();
                                for (int i = 0; i < openDistribution.Length; i++)
                                {
                                    //bpsUM.AnalysisResult.AcousticResult.AcousticUIOutput.LossDistributions[0][0].Frequency = bpsUM_v1.AnalysisResult.AcousticResult.AcousticUIOutput.LossDistributions[0][0].Frequency;
                                    //bpsUM.AnalysisResult.AcousticResult.AcousticUIOutput.LossDistributions[0][0].Tau = bpsUM_v1.AnalysisResult.AcousticResult.AcousticUIOutput.LossDistributions[0][0].Tau;
                                    //bpsUM.AnalysisResult.AcousticResult.AcousticUIOutput.LossDistributions[0][0].STL = bpsUM_v1.AnalysisResult.AcousticResult.AcousticUIOutput.LossDistributions[0][0].STL;
                                    bpsUM.AnalysisResult.AcousticResult.AcousticUIOutput.LossDistributions.Add(openDistribution);
                                }
                                bpsUM.AnalysisResult.AcousticResult.AcousticUIOutput.TotalRw = bpsUM_v1.AnalysisResult.AcousticResult.AcousticUIOutput.TotalRw;
                                #endregion
                                bpsUM.AnalysisResult.AcousticResult.reportFileUrl = bpsUM_v1.AnalysisResult.AcousticResult.reportFileUrl;
                            }
                            #endregion

                            /* --- 2 Structural Result --- */
                            #region StructuralResult
                            if (bpsUM_v1.AnalysisResult.StructuralResult != null)
                            {
                                bpsUM.AnalysisResult.StructuralResult = new BpsUnifiedModelLib.StructuralResult();
                                #region MemberResults
                                if (bpsUM_v1.AnalysisResult.StructuralResult.MemberResults != null)
                                {
                                    bpsUM.AnalysisResult.StructuralResult.MemberResults = new List<BpsUnifiedModelLib.StructuralMemberResult>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.StructuralResult.MemberResults)
                                    {
                                        var structuralMemberResult = new BpsUnifiedModelLib.StructuralMemberResult();
                                        structuralMemberResult.memberID = item.memberID;
                                        structuralMemberResult.deflectionRatio = item.deflectionRatio;
                                        structuralMemberResult.verticalDeflectionRatio = item.verticalDeflectionRatio;
                                        structuralMemberResult.stressRatio = item.stressRatio;
                                        structuralMemberResult.shearRatio = item.shearRatio;
                                        bpsUM.AnalysisResult.StructuralResult.MemberResults.Add(structuralMemberResult);
                                    }
                                }
                                #endregion
                                bpsUM.AnalysisResult.StructuralResult.reportFileUrl = bpsUM_v1.AnalysisResult.StructuralResult.reportFileUrl;
                                bpsUM.AnalysisResult.StructuralResult.summaryFileUrl = bpsUM_v1.AnalysisResult.StructuralResult.summaryFileUrl;
                                bpsUM.AnalysisResult.StructuralResult.errorMessage = bpsUM_v1.AnalysisResult.StructuralResult.errorMessage;
                            }
                            #endregion

                            /* --- 3 Facade Structural Result --- */
                            #region FacadeStructuralResult
                            if (bpsUM_v1.AnalysisResult.FacadeStructuralResult != null)
                            {
                                bpsUM.AnalysisResult.FacadeStructuralResult = new BpsUnifiedModelLib.FacadeStructuralResult();
                                #region MemberResults
                                if (bpsUM_v1.AnalysisResult.FacadeStructuralResult.MemberResults != null)
                                {
                                    bpsUM.AnalysisResult.FacadeStructuralResult.MemberResults = new List<BpsUnifiedModelLib.FacadeStructuralMemberResult>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.FacadeStructuralResult.MemberResults)
                                    {
                                        var facadeStructuralMemberResult = new BpsUnifiedModelLib.FacadeStructuralMemberResult();
                                        facadeStructuralMemberResult.memberID = item.memberID;
                                        facadeStructuralMemberResult.outofplaneBendingCapacityRatio = item.outofplaneBendingCapacityRatio;
                                        facadeStructuralMemberResult.outofplaneReinfBendingCapacityRatio = item.outofplaneReinfBendingCapacityRatio;
                                        facadeStructuralMemberResult.inplaneBendingCapacityRatio = item.inplaneBendingCapacityRatio;
                                        facadeStructuralMemberResult.outofplaneDeflectionCapacityRatio = item.outofplaneDeflectionCapacityRatio;
                                        facadeStructuralMemberResult.inplaneDeflectionCapacityRatio = item.inplaneDeflectionCapacityRatio;
                                        bpsUM.AnalysisResult.FacadeStructuralResult.MemberResults.Add(facadeStructuralMemberResult);
                                    }
                                }
                                #endregion
                                bpsUM.AnalysisResult.FacadeStructuralResult.reportFileUrl = bpsUM_v1.AnalysisResult.FacadeStructuralResult.reportFileUrl;
                                bpsUM.AnalysisResult.FacadeStructuralResult.summaryFileUrl = bpsUM_v1.AnalysisResult.FacadeStructuralResult.summaryFileUrl;
                                bpsUM.AnalysisResult.FacadeStructuralResult.errorMessage = bpsUM_v1.AnalysisResult.FacadeStructuralResult.errorMessage;
                            }
                            #endregion

                            /* --- 4 UDC Structural Result --- */
                            #region UDCStructuralResult
                            if (bpsUM_v1.AnalysisResult.UDCStructuralResult != null)
                            {
                                bpsUM.AnalysisResult.UDCStructuralResult = new BpsUnifiedModelLib.UDCStructuralResult();
                                #region MemberResults
                                if (bpsUM_v1.AnalysisResult.UDCStructuralResult.MemberResults != null)
                                {
                                    bpsUM.AnalysisResult.UDCStructuralResult.MemberResults = new List<BpsUnifiedModelLib.UDCStructuralMemberResult>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.UDCStructuralResult.MemberResults)
                                    {
                                        var udcStructuralMemberResult = new BpsUnifiedModelLib.UDCStructuralMemberResult();
                                        udcStructuralMemberResult.memberID = item.memberID;
                                        udcStructuralMemberResult.outofplaneBendingCapacityRatio = item.outofplaneBendingCapacityRatio;
                                        udcStructuralMemberResult.inplaneBendingCapacityRatio = item.inplaneBendingCapacityRatio;
                                        udcStructuralMemberResult.outofplaneDeflectionCapacityRatio = item.outofplaneDeflectionCapacityRatio;
                                        udcStructuralMemberResult.inplaneDeflectionCapacityRatio = item.inplaneDeflectionCapacityRatio;
                                        bpsUM.AnalysisResult.UDCStructuralResult.MemberResults.Add(udcStructuralMemberResult);
                                    }
                                }
                                #endregion
                                bpsUM.AnalysisResult.UDCStructuralResult.reportFileUrl = bpsUM_v1.AnalysisResult.UDCStructuralResult.reportFileUrl;
                                bpsUM.AnalysisResult.UDCStructuralResult.summaryFileUrl = bpsUM_v1.AnalysisResult.UDCStructuralResult.summaryFileUrl;
                            }
                            #endregion

                            /* --- 5 Thermal Result --- */
                            #region ThermalResult
                            if (bpsUM_v1.AnalysisResult.ThermalResult != null)
                            {
                                bpsUM.AnalysisResult.ThermalResult = new BpsUnifiedModelLib.ThermalResult();
                                bpsUM.AnalysisResult.ThermalResult.reportFileUrl = bpsUM_v1.AnalysisResult.ThermalResult.reportFileUrl;
                                #region ThermalUIResult
                                bpsUM.AnalysisResult.ThermalResult.ThermalUIResult = new BpsUnifiedModelLib.ThermalOutput();
                                bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.TotalArea = bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.TotalArea;
                                bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.TotalUw = bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.TotalUw;
                                #region ThermalFrames
                                if (bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalFrames != null)
                                {

                                    bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalFrames = new List<BpsUnifiedModelLib.ThermalOutput.ThermalFrame>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalFrames)
                                    {
                                        var thermalFrame = new BpsUnifiedModelLib.ThermalOutput.ThermalFrame();
                                        #region FrameSegs
                                        if (item.FrameSegs != null)
                                        {
                                            thermalFrame.FrameSegs = new List<BpsUnifiedModelLib.ThermalOutput.FrameSegment>();
                                            foreach (var subItem in item.FrameSegs)
                                            {
                                                var frameSeg = new BpsUnifiedModelLib.ThermalOutput.FrameSegment();
                                                frameSeg.PointA = new BpsUnifiedModelLib.Point();
                                                frameSeg.PointB = new BpsUnifiedModelLib.Point();
                                                frameSeg.FrameSegID = subItem.FrameSegID;
                                                frameSeg.PointA.PointID = subItem.PointA.PointID;
                                                frameSeg.PointA.X = subItem.PointA.X;
                                                frameSeg.PointA.Y = subItem.PointA.Y;
                                                frameSeg.PointB.PointID = subItem.PointB.PointID;
                                                frameSeg.PointB.X = subItem.PointB.X;
                                                frameSeg.PointB.Y = subItem.PointB.Y;
                                                frameSeg.ArticleCombo = subItem.ArticleCombo;
                                                thermalFrame.FrameSegs.Add(frameSeg);
                                            }
                                        }
                                        #endregion
                                        thermalFrame.Area = item.Area;
                                        thermalFrame.Uf = item.Uf;
                                        thermalFrame.UfNote = item.UfNote;
                                        thermalFrame.ThermalFrameID = item.ThermalFrameID;
                                        bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalFrames.Add(thermalFrame);
                                    }
                                }
                                #endregion
                                #region ThermalFacadeMembers
                                if (bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalFacadeMembers != null)
                                {
                                    bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalFacadeMembers = new List<BpsUnifiedModelLib.ThermalOutput.ThermalFacadeMember>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalFacadeMembers)
                                    {
                                        var thermalFacadeMember = new BpsUnifiedModelLib.ThermalOutput.ThermalFacadeMember();
                                        thermalFacadeMember.ArticleID = item.ArticleID;
                                        thermalFacadeMember.Area = item.Area;
                                        thermalFacadeMember.Uf = item.Uf;
                                        thermalFacadeMember.HeatLoss = item.HeatLoss;
                                        thermalFacadeMember.FacadeFrameID = item.FacadeFrameID;
                                        thermalFacadeMember.Width = item.Width;
                                        bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalFacadeMembers.Add(thermalFacadeMember);
                                    }
                                }
                                #endregion
                                #region ThermalUIFacadeGlassEdges
                                //ThermalOutput.ThermalUIFacadeGlassEdges;
                                #endregion
                                #region ThermalUIInsertUnitGlasses
                                if (bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitGlasses != null)
                                {
                                    bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitGlasses = new List<BpsUnifiedModelLib.ThermalOutput.ThermalGlass>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitGlasses)
                                    {
                                        var obj = new BpsUnifiedModelLib.ThermalOutput.ThermalGlass();
                                        obj.GlassID = item.GlassID;
                                        obj.Ug = item.Ug;
                                        obj.Area = item.Area;
                                        bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitGlasses.Add(obj);
                                    }
                                }
                                #endregion
                                #region ThermalUIGlasses
                                if (bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIGlasses != null)
                                {
                                    bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIGlasses = new List<BpsUnifiedModelLib.ThermalOutput.ThermalUIGlass>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIGlasses)
                                    {
                                        var obj = new BpsUnifiedModelLib.ThermalOutput.ThermalUIGlass();
                                        obj.GlassID = item.GlassID;
                                        obj.Ug = item.Ug;
                                        obj.Area = item.Area;
                                        bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIGlasses.Add(obj);
                                    }
                                }
                                #endregion
                                #region ThermalUIInsertUnitPanels
                                if (bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitPanels != null)
                                {
                                    bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitPanels = new List<BpsUnifiedModelLib.ThermalOutput.ThermalPanel>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitPanels)
                                    {
                                        var obj = new BpsUnifiedModelLib.ThermalOutput.ThermalPanel();
                                        obj.GlassID = item.GlassID;
                                        obj.Up = item.Up;
                                        obj.Area = item.Area;
                                        bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitPanels.Add(obj);
                                    }
                                }
                                #endregion
                                #region ThermalUIPanels
                                if (bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIPanels != null)
                                {
                                    bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIPanels = new List<BpsUnifiedModelLib.ThermalOutput.ThermalUIPanel>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIPanels)
                                    {
                                        var obj = new BpsUnifiedModelLib.ThermalOutput.ThermalUIPanel();
                                        obj.GlassID = item.GlassID;
                                        obj.Up = item.Up;
                                        obj.Area = item.Area;
                                        bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIPanels.Add(obj);
                                    }
                                }
                                #endregion
                                #region ThermalUIGlassEdges
                                if (bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIGlassEdges != null)
                                {
                                    bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIGlassEdges = new List<BpsUnifiedModelLib.ThermalOutput.ThermalUIGlassEdge>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIGlassEdges)
                                    {
                                        var obj = new BpsUnifiedModelLib.ThermalOutput.ThermalUIGlassEdge();
                                        obj.GlassID = item.GlassID;
                                        obj.Psi = item.Psi;
                                        obj.Length = item.Length;
                                        bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIGlassEdges.Add(obj);
                                    }
                                }
                                #endregion
                                #region ThermalUIFacadeGlassEdges
                                if (bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIFacadeGlassEdges != null)
                                {
                                    bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIFacadeGlassEdges = new List<BpsUnifiedModelLib.ThermalOutput.ThermalUIFacadeGlassEdge>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIFacadeGlassEdges)
                                    {
                                        var obj = new BpsUnifiedModelLib.ThermalOutput.ThermalUIFacadeGlassEdge();
                                        obj.GlassID = item.GlassID;
                                        obj.PsiH = item.PsiH;
                                        obj.PsiV = item.PsiV;
                                        obj.LengthH = item.LengthH;
                                        obj.LengthV = item.LengthV;
                                        bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIFacadeGlassEdges.Add(obj);
                                    }
                                }
                                #endregion
                                #region ThermalUIInsertUnitPanelEdges
                                if (bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitPanelEdges != null)
                                {
                                    bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitPanelEdges = new List<BpsUnifiedModelLib.ThermalOutput.ThermalPanelEdge>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitPanelEdges)
                                    {
                                        var obj = new BpsUnifiedModelLib.ThermalOutput.ThermalPanelEdge();
                                        obj.GlassID = item.GlassID;
                                        obj.PanelDiscript = item.PanelDiscript;
                                        obj.Psi = item.Psi;
                                        obj.Length = item.Length;
                                        obj.HeatLoss = item.HeatLoss;
                                        bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitPanelEdges.Add(obj);
                                    }
                                }
                                #endregion
                                #region ThermalUIPanelEdges
                                if (bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIPanelEdges != null)
                                {
                                    bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIPanelEdges = new List<BpsUnifiedModelLib.ThermalOutput.ThermalUIPanelEdge>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIPanelEdges)
                                    {
                                        var thermalUIPanelEdge = new BpsUnifiedModelLib.ThermalOutput.ThermalUIPanelEdge();
                                        thermalUIPanelEdge.GlassID = item.GlassID;
                                        thermalUIPanelEdge.Psi = item.Psi;
                                        thermalUIPanelEdge.Length = item.Length;
                                        bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIPanelEdges.Add(thermalUIPanelEdge);
                                    }
                                }
                                #endregion
                                #region ThermalUIInsertUnitGlassEdges
                                if (bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitGlassEdges != null)
                                {
                                    bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitGlassEdges = new List<BpsUnifiedModelLib.ThermalOutput.ThermalGlassEdge>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitGlassEdges)
                                    {
                                        var thermalGlassEdge = new BpsUnifiedModelLib.ThermalOutput.ThermalGlassEdge();
                                        thermalGlassEdge.GlassID = item.GlassID;
                                        thermalGlassEdge.Psi = item.Psi;
                                        thermalGlassEdge.Length = item.Length;
                                        bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitGlassEdges.Add(thermalGlassEdge);
                                    }
                                }
                                #endregion
                                #region ThermalUIInsertUnitFrameEdges
                                if (bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitFrameEdges != null)
                                {
                                    bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitFrameEdges = new List<BpsUnifiedModelLib.ThermalOutput.ThermalUIInsertUnitFrameEdge>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitFrameEdges)
                                    {
                                        var thermalUIInsertUnitFrameEdge = new BpsUnifiedModelLib.ThermalOutput.ThermalUIInsertUnitFrameEdge();
                                        thermalUIInsertUnitFrameEdge.GlassID = item.GlassID;
                                        thermalUIInsertUnitFrameEdge.Psi = item.Psi;
                                        thermalUIInsertUnitFrameEdge.Length = item.Length;
                                        bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitFrameEdges.Add(thermalUIInsertUnitFrameEdge);
                                    }
                                }
                                #endregion
                                #region ThermalUIInsertUnitFrames
                                if (bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitFrames != null)
                                {
                                    bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitFrames = new List<BpsUnifiedModelLib.ThermalOutput.ThermalInsertUnitFrame>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitFrames)
                                    {
                                        var thermalInsertUnitFrame = new BpsUnifiedModelLib.ThermalOutput.ThermalInsertUnitFrame();
                                        thermalInsertUnitFrame.GlassID = item.GlassID;
                                        thermalInsertUnitFrame.ArticleIDCombo = item.ArticleIDCombo;
                                        thermalInsertUnitFrame.Uf = item.Uf;
                                        thermalInsertUnitFrame.Area = item.Area;
                                        bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.ThermalUIInsertUnitFrames.Add(thermalInsertUnitFrame);
                                    }
                                }
                                #endregion
                                #region GlassGeometricInfos
                                if (bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.GlassGeometricInfos != null)
                                {
                                    bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.GlassGeometricInfos = new List<BpsUnifiedModelLib.ThermalOutput.GlassGeometricInfo>();
                                    foreach (var item in bpsUM_v1.AnalysisResult.ThermalResult.ThermalUIResult.GlassGeometricInfos)
                                    {
                                        var glassGeometricInfo = new BpsUnifiedModelLib.ThermalOutput.GlassGeometricInfo();
                                        glassGeometricInfo.GlassID = item.GlassID;
                                        glassGeometricInfo.PointCoordinates = item.PointCoordinates;
                                        glassGeometricInfo.CornerCoordinates = item.CornerCoordinates;
                                        glassGeometricInfo.VentCoordinates = item.VentCoordinates;
                                        glassGeometricInfo.InsertOuterFrameCoordinates = item.InsertOuterFrameCoordinates;
                                        glassGeometricInfo.VentOpeningDirection = item.VentOpeningDirection;
                                        glassGeometricInfo.VentOperableType = item.VentOperableType;
                                        bpsUM.AnalysisResult.ThermalResult.ThermalUIResult.GlassGeometricInfos.Add(glassGeometricInfo);
                                    }
                                }
                                #endregion
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                        return JsonConvert.SerializeObject(bpsUM);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getV2UnifiedModelManageUpdate(string bpsUM_v2_string)
        {
            try
            {
                BpsUnifiedModel bpsUM_v1 = new BpsUnifiedModel();
                try
                {
                    bpsUM_v1 = JsonConvert.DeserializeObject<BpsUnifiedModel>(bpsUM_v2_string);
                    var systemType = bpsUM_v1.ModelInput.FrameSystem.SystemType.Split('.');
                    try
                    {
                        if (bpsUM_v1.ProblemSetting.ProblemGuid.ToString() == "3f88d9eb-ca9f-4621-887d-348b1d7f7acf") { 

                        }
                        if (bpsUM_v1.ProblemSetting.ProductType == "Facade")
                        {
                            bpsUM_v1.ModelInput.FrameSystem.SystemType = systemType[0];
                            if (bpsUM_v1.ProblemSetting.FacadeType == "UDC" || bpsUM_v1.ProblemSetting.FacadeType == "mullion-transom")
                            {
                                if (bpsUM_v1.ProblemSetting.FacadeType == "mullion-transom")
                                {
                                    if (bpsUM_v1.ModelInput.Geometry.PanelSystems != null)
                                        foreach (var item in bpsUM_v1.ModelInput.Geometry.PanelSystems)
                                        {
                                            if (item.PanelID == 0) {
                                                switch (item.Description) {
                                                    case "2/20/2 (24 mm)": item.PanelID = 1; break;
                                                    case "6/20/2 (28 mm)": item.PanelID = 2; break;
                                                    case "2/40/2 (44 mm)": item.PanelID = 3; break;
                                                    case "6/40/2 (48 mm)": item.PanelID = 4; break;
                                                    case "2/100/2 (104 mm)": item.PanelID = 5; break;
                                                    case "6/100/2 (108 mm)": item.PanelID = 6; break;
                                                    case "2/26/120/2 (150 mm)": item.PanelID = 7; break;
                                                    case "6/22/120/2 (150 mm)": item.PanelID = 8; break;
                                                    case "2/220/2 (224 mm)": item.PanelID = 9; break;
                                                    case "6/220/2 (228 mm)": item.PanelID = 10; break;
                                                    default: item.PanelID = 0; break;
                                                }        
                                            }
                                        }
                                }

                                if (systemType.Length > 1)
                                {
                                    if (systemType[0] == "FWS 50" && (systemType[1] == "SI" || systemType[1] == "SI GREEN"))
                                        bpsUM_v1.ModelInput.FrameSystem.InsulationZone = "";
                                    else
                                        bpsUM_v1.ModelInput.FrameSystem.InsulationZone = systemType[1];
                                }
                                else {
                                    if (bpsUM_v1.ModelInput.FrameSystem.SystemType == "FWS 50" && (bpsUM_v1.ModelInput.FrameSystem.InsulationZone == "SI" || bpsUM_v1.ModelInput.FrameSystem.InsulationZone == "SI GREEN"))
                                        bpsUM_v1.ModelInput.FrameSystem.InsulationZone = "";
                                }
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            bpsUM_v1.ProblemSetting.ProductType = "Window";
                            bpsUM_v1.ModelInput.FrameSystem.SystemType = bpsUM_v1.ModelInput.FrameSystem.SystemType + (bpsUM_v1.ModelInput.FrameSystem.InsulationZone == null ? "" : ("." + bpsUM_v1.ModelInput.FrameSystem.InsulationZone));
                            bpsUM_v1.ModelInput.FrameSystem.InsulationZone = null;
                            bpsUM_v1.ProblemSetting.FacadeType = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    return JsonConvert.SerializeObject(bpsUM_v1);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string oldToNewArticle(string oldArticle)
        {
            if (oldArticle == "322250") return "536800";
            else if (oldArticle == "322260") return "536810";
            else if (oldArticle == "322270") return "536820";
            else if (oldArticle == "322280") return "536830";
            else if (oldArticle == "322280") return "536720";
            else if (oldArticle == "322290") return "536840";
            else if (oldArticle == "322300") return "536850";
            else if (oldArticle == "322310") return "536270";
            else if (oldArticle == "322310") return "536320";
            else if (oldArticle == "326030") return "536380";
            else if (oldArticle == "336230") return "536470";
            else if (oldArticle == "336240") return "536180";
            else if (oldArticle == "324010") return "543000";
            else if (oldArticle == "324020") return "543010";
            else if (oldArticle == "324030") return "543020";
            else if (oldArticle == "324040") return "543030";
            else if (oldArticle == "324050") return "543040";
            else if (oldArticle == "324060") return "543050";
            else if (oldArticle == "324070") return "536540";
            else if (oldArticle == "324080") return "536580";
            else if (oldArticle == "324090") return "536620";
            else if (oldArticle == "336270") return "536660";
            //Facade Mullion Reinforcement
            else if (oldArticle == "336280") return "536690";
            else if (oldArticle == "324360") return "536650";
            else if (oldArticle == "324350") return "536610";
            else if (oldArticle == "324340") return "536570";
            else return oldArticle;
        }

        protected int getArticleDepth(string articleName) 
        {
            int result = 0;
            List<Article> articles = completeArticlesList();
            foreach (var article in articles)
            { 
                if(article.OLD == articleName || article.NEW == articleName)
                    result = article.DEPTH;
            }
            return result;
        }
        protected List<Article> completeArticlesList()
        {
            var articlesList = new List<Article>();
            articlesList.Add(setArticle(1, "FWS 35", "MULLION", "477750", "477750", 65));
            articlesList.Add(setArticle(2, "FWS 35", "MULLION", "477760", "477760", 85));
            articlesList.Add(setArticle(3, "FWS 35", "MULLION", "477770", "477770", 105));
            articlesList.Add(setArticle(4, "FWS 35", "MULLION", "477780", "477780", 125));
            articlesList.Add(setArticle(5, "FWS 35", "MULLION", "477790", "477790", 150));
            articlesList.Add(setArticle(1, "FWS 50", "MULLION", "322250", "536800", 50));
            articlesList.Add(setArticle(2, "FWS 50", "MULLION", "322260", "536810", 65));
            articlesList.Add(setArticle(3, "FWS 50", "MULLION", "322270", "536820", 85));
            articlesList.Add(setArticle(4, "FWS 50", "MULLION", "322280", "536830", 105));
            articlesList.Add(setArticle(5, "FWS 50", "MULLION", "536720", "536720", 115));
            articlesList.Add(setArticle(6, "FWS 50", "MULLION", "322290", "536840", 125));
            articlesList.Add(setArticle(7, "FWS 50", "MULLION", "322300", "536850", 150));
            articlesList.Add(setArticle(8, "FWS 50", "MULLION", "322310", "536270", 175));
            articlesList.Add(setArticle(9, "FWS 50", "MULLION", "536320", "536320", 175));
            articlesList.Add(setArticle(10, "FWS 50", "MULLION", "326030", "536380", 200));
            articlesList.Add(setArticle(11, "FWS 50", "MULLION", "336230", "536470", 225));
            articlesList.Add(setArticle(12, "FWS 50", "MULLION", "336240", "536180", 250));
            articlesList.Add(setArticle(1, "FWS 60", "MULLION", "324010", "543000", 50));
            articlesList.Add(setArticle(2, "FWS 60", "MULLION", "324020", "543010", 65));
            articlesList.Add(setArticle(3, "FWS 60", "MULLION", "324030", "543020", 85));
            articlesList.Add(setArticle(4, "FWS 60", "MULLION", "324040", "543030", 105));
            articlesList.Add(setArticle(5, "FWS 60", "MULLION", "324050", "543040", 125));
            articlesList.Add(setArticle(6, "FWS 60", "MULLION", "324060", "543050", 150));
            articlesList.Add(setArticle(7, "FWS 60", "MULLION", "324070", "536540", 175));
            articlesList.Add(setArticle(8, "FWS 60", "MULLION", "324080", "536580", 200));
            articlesList.Add(setArticle(9, "FWS 60", "MULLION", "324090", "536620", 225));
            articlesList.Add(setArticle(10, "FWS 60", "MULLION", "336270", "536660", 250));

            articlesList.Add(setArticle(1, "FWS 35", "TRANSOM", "477800", "477800", 70));
            articlesList.Add(setArticle(2, "FWS 35", "TRANSOM", "477810", "477810", 90));
            articlesList.Add(setArticle(3, "FWS 35", "TRANSOM", "477820", "477820", 110));
            articlesList.Add(setArticle(4, "FWS 35", "TRANSOM", "477830", "477830", 130));
            articlesList.Add(setArticle(5, "FWS 35", "TRANSOM", "477840", "477840", 155));
            articlesList.Add(setArticle(1, "FWS 50", "TRANSOM", "322370", "322370", 6));
            articlesList.Add(setArticle(2, "FWS 50", "TRANSOM", "322380", "322380", 21));
            articlesList.Add(setArticle(3, "FWS 50", "TRANSOM", "322460", "322460", 27));
            articlesList.Add(setArticle(4, "FWS 50", "TRANSOM", "323840", "323840", 45));
            articlesList.Add(setArticle(5, "FWS 50", "TRANSOM", "322390", "322390", 55));
            articlesList.Add(setArticle(6, "FWS 50", "TRANSOM", "322400", "322400", 70));
            articlesList.Add(setArticle(7, "FWS 50", "TRANSOM", "322410", "322410", 90));
            articlesList.Add(setArticle(8, "FWS 50", "TRANSOM", "322420", "322420", 110));
            articlesList.Add(setArticle(9, "FWS 50", "TRANSOM", "536750", "536750", 120));
            articlesList.Add(setArticle(10, "FWS 50", "TRANSOM", "322430", "322430", 130));
            articlesList.Add(setArticle(11, "FWS 50", "TRANSOM", "322440", "322440", 155));
            articlesList.Add(setArticle(12, "FWS 50", "TRANSOM", "322450", "322450", 180));
            articlesList.Add(setArticle(13, "FWS 50", "TRANSOM", "449590", "449590", 205));
            articlesList.Add(setArticle(14, "FWS 50", "TRANSOM", "505020", "505020", 230));
            articlesList.Add(setArticle(15, "FWS 50", "TRANSOM", "449600", "449600", 230));
            articlesList.Add(setArticle(16, "FWS 50", "TRANSOM", "505030", "505030", 255));
            articlesList.Add(setArticle(17, "FWS 50", "TRANSOM", "449610", "449610", 255));
            articlesList.Add(setArticle(1, "FWS 60", "TRANSOM", "324400", "324400", 6));
            articlesList.Add(setArticle(2, "FWS 60", "TRANSOM", "324410", "324410", 21));
            articlesList.Add(setArticle(3, "FWS 60", "TRANSOM", "324420", "324420", 27));
            articlesList.Add(setArticle(4, "FWS 60", "TRANSOM", "324430", "324430", 45));
            articlesList.Add(setArticle(5, "FWS 60", "TRANSOM", "324440", "324440", 55));
            articlesList.Add(setArticle(6, "FWS 60", "TRANSOM", "324450", "324450", 70));
            articlesList.Add(setArticle(7, "FWS 60", "TRANSOM", "324460", "324460", 90));
            articlesList.Add(setArticle(8, "FWS 60", "TRANSOM", "324470", "324470", 110));
            articlesList.Add(setArticle(9, "FWS 60", "TRANSOM", "324480", "324480", 130));
            articlesList.Add(setArticle(10, "FWS 60", "TRANSOM", "324490", "324490", 155));
            articlesList.Add(setArticle(11, "FWS 60", "TRANSOM", "324500", "324500", 180));
            articlesList.Add(setArticle(12, "FWS 60", "TRANSOM", "326940", "326940", 205));
            articlesList.Add(setArticle(13, "FWS 60", "TRANSOM", "493680", "493680", 230));
            articlesList.Add(setArticle(14, "FWS 60", "TRANSOM", "493690", "493690", 255));


            return articlesList;
        }
        public Article setArticle(int SN, string SYSTEM, string TYPE, string OLD, string NEW, int DEPTH)
        {
            Article article = new Article();
            article.SN = SN;
            article.SYSTEM = SYSTEM;
            article.TYPE = TYPE;
            article.OLD = OLD;
            article.NEW = NEW;
            article.DEPTH = DEPTH;
            return article;
        }
        public class Article
        {
            public int SN;
            public string SYSTEM;
            public string TYPE;
            public string OLD;
            public string NEW;
            public int DEPTH;
            
        }

    }
}
