using BpsUnifiedModelLib;
using SBA;
using StructuralSolverSBA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VCLWebAPI.Models.Edmx;

namespace VCLWebAPI.Services
{
    public class StructuralService
    {
        private readonly VCLDesignDBEntities _db;

        public StructuralService()
        {
            _db = new VCLDesignDBEntities();
        }

        public WindZoneOutput GetWindZone(string PostCode)
        {
            WindZoneGermany windzoneGermany = _db.WindZoneGermany.Where(x => x.PostCode == PostCode).SingleOrDefault();
            int windzone = 0;
            string state = string.Empty, district = string.Empty, place = string.Empty;
            double vb0 = 0.0, qb0 = 0.0;

            if (!(windzoneGermany is null))
            {
                windzone = windzoneGermany.WindZone;
                state = windzoneGermany.State;
                district = windzoneGermany.District;
                place = windzoneGermany.Place;
                switch (windzone)       // NA.A.1
                {
                    case 1:
                        vb0 = 22.5;     //m/s
                        qb0 = 0.32;     //kN/m2
                        break;

                    case 2:
                        vb0 = 25;
                        qb0 = 0.39;
                        break;

                    case 3:
                        vb0 = 27.5;
                        qb0 = 0.47;
                        break;

                    case 4:
                        vb0 = 30;
                        qb0 = 0.56;
                        break;
                }
            }

            WindZoneOutput windzoneOutput = new WindZoneOutput
            {
                WindZone = windzone,
                State = state,
                District = district,
                Place = place,
                vb0 = vb0,
                qb0 = qb0
            };
            return windzoneOutput;
        }

        public WindLoadOutput CalculateWindLoadDIN(DinWindLoadInput input)
        {
            DINWindLoadCalculation WLC = new DINWindLoadCalculation(input);
            WindLoadOutput wlOutput = WLC.Calculate();

            // Clear out that string because we dont need it in the UI
            wlOutput.WLCString = input.RequestDescription ? wlOutput.WLCString : String.Empty;

            return wlOutput;
        }

        // calculate sectional properties from uploaded dxf
        public Section CalculateDXFSectionProperties(DXFInput dxfInput)
        {
            string dxfFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\temp\temp.dxf");
            File.Delete(dxfFilePath);
            using (StreamWriter tempFile = new StreamWriter(dxfFilePath))
            {
                tempFile.Write(dxfInput.content);
            }

            Section sectionOutput = new Section();
            string customArticle = dxfInput.fileName;
            sectionOutput.ArticleName = customArticle.Substring(0, customArticle.Length - 4);

            var DSP = new DXFSectionProperty();
            DSP.ReadDXF(dxfFilePath, ref sectionOutput);
            File.Delete(dxfFilePath);
            return sectionOutput;
        }

        // main entry for ReadSectionProperties
        public void ReadSectionPropertiesFromDB(BpsUnifiedModel unifiedModel)
        {
            if (unifiedModel.ProblemSetting.ProductType == "Facade" && unifiedModel.ProblemSetting.FacadeType == "mullion-transom")
            {
                ReadFacadeSectionPropertiesFromDB(unifiedModel);
            }
            else if (unifiedModel.ProblemSetting.ProductType == "Facade" && unifiedModel.ProblemSetting.FacadeType == "UDC")
            {
                ReadFacadeSectionPropertiesFromDB(unifiedModel);
            }
            else if (unifiedModel.ProblemSetting.ProductType == "Window")
            {
                ReadWindowSectionPropertiesFromDB(unifiedModel);
            }
        }

        // main entry for ReadSectionPropertiesFromDXF
        public void ReadSectionPropertiesFromDXF(BpsUnifiedModel unifiedModel)
        {
            if (unifiedModel.ProblemSetting.ProductType == "Facade" && unifiedModel.ProblemSetting.FacadeType == "mullion-transom")
            {
                ReadFacadeSectionProperties(unifiedModel);
            }
            else if (unifiedModel.ProblemSetting.ProductType == "Facade" && unifiedModel.ProblemSetting.FacadeType == "UDC")
            {
                ReadFacadeSectionProperties(unifiedModel);
            }
            else if (unifiedModel.ProblemSetting.ProductType == "Window")
            {
                ReadWindowSectionProperties(unifiedModel);
            }
        }

        // read window profile from dxf
        public void ReadWindowSectionProperties(BpsUnifiedModel unifiedModel)
        {
            List<Section> sections = unifiedModel.ModelInput.Geometry.Sections;
            for (int i = 0; i < sections.Count(); i++)
            {
                Section section = sections[i];
                string dxfFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\dxf\", $"{section.ArticleName}.dxf");
                if (!section.isCustomProfile)
                {
                    var DSP = new DXFSectionProperty();
                    DSP.ReadDXF(dxfFilePath, ref section);
                }
            }
            // update material here
            UpdateMaterialProperties(unifiedModel);
        }

        // read window profile from DB
        public void ReadWindowSectionPropertiesFromDB(BpsUnifiedModel unifiedModel)
        {
            List<Section> sections = unifiedModel.ModelInput.Geometry.Sections.Where(x => x.SectionType == 1 || x.SectionType == 2 || x.SectionType == 3).ToList(); // need confirm with Grace

            Dictionary<string, Section> articleNumSectionMap = new Dictionary<string, Section>();

            for (int i = 0; i < sections.Count(); i++)
            {
                Section section = sections[i];

                // Load result if already been calculated
                if (articleNumSectionMap.ContainsKey(section.ArticleName))
                {
                    LoadSectionResult(articleNumSectionMap[section.ArticleName], ref section);
                }
                else
                {
                    if (section.isCustomProfile)
                    {
                        articleNumSectionMap[section.ArticleName] = section;
                        continue;
                    }

                    ArticleSectionalProperty dbArticleSectionalProperty = _db.ArticleSectionalProperty.Where(x => x.ArticleName == section.ArticleName).SingleOrDefault();
                    if (dbArticleSectionalProperty == null)
                    {
                        throw new InvalidDataException();
                    }
                    else
                    {
                        LoadSectionPropertyFromDB(dbArticleSectionalProperty, ref section);
                        articleNumSectionMap[section.ArticleName] = section;
                    }
                }
            }
            // update material here
            UpdateMaterialProperties(unifiedModel);
        }

        internal void LoadSectionResult(Section result, ref Section target)
        {
            target.InsideW = result.InsideW;
            target.OutsideW = result.OutsideW;
            target.LeftRebate = result.LeftRebate;
            target.RightRebate = result.RightRebate;
            target.DistBetweenIsoBars = result.DistBetweenIsoBars;
            target.d = result.d;
            target.Weight = result.Weight;
            target.Ao = result.Ao;
            target.Au = result.Au;
            target.Io = result.Io;
            target.Iu = result.Iu;
            target.Ioyy = result.Ioyy;
            target.Iuyy = result.Iuyy;
            target.Zoo = result.Zoo;
            target.Zou = result.Zou;
            target.Zol = result.Zol;
            target.Zor = result.Zor;
            target.Zuo = result.Zuo;
            target.Zuu = result.Zuu;
            target.Zul = result.Zul;
            target.Zur = result.Zur;
            target.RSn20 = result.RSn20;
            target.RSp80 = result.RSp80;
            target.RTn20 = result.RTn20;
            target.RTp80 = result.RTp80;
            target.Cn20 = result.Cn20;
            target.Cp20 = result.Cp20;
            target.Cp80 = result.Cp80;
            target.beta = result.beta;
            target.A2 = result.A2;
            target.E = result.E;
            target.alpha = result.alpha;
            target.Woyp = result.Woyp;
            target.Woyn = result.Woyn;
            target.Wozp = result.Wozp;
            target.Wozn = result.Wozn;
            target.Wuyp = result.Wuyp;
            target.Wuyn = result.Wuyn;
            target.Wuzp = result.Wuzp;
            target.Wuzn = result.Wuzn;
            target.Depth = result.Depth;
        }

        internal void LoadSectionPropertyFromDB(ArticleSectionalProperty result, ref Section target)
        {
            //target.InsideW = result.InsideW;  // these fields should come from another table
            //target.OutsideW = result.OutsideW;
            target.LeftRebate = result.LeftRebate;
            target.RightRebate = result.RightRebate;
            //target.DistBetweenIsoBars = result.DistBetweenIsoBars;
            target.d = result.d;
            target.Weight = result.Weight;
            target.Ao = result.Ao;
            target.Au = result.Au;
            target.Io = result.Io;
            target.Iu = result.Iu;
            target.Ioyy = result.Ioyy;
            target.Iuyy = result.Iuyy;
            target.Zoo = result.Zoo;
            target.Zou = result.Zou;
            target.Zol = result.Zol;
            target.Zor = result.Zor;
            target.Zuo = result.Zuo;
            target.Zuu = result.Zuu;
            target.Zul = result.Zul;
            target.Zur = result.Zur;
            target.RSn20 = result.RSn20;
            target.RSp80 = result.RSp80;
            target.RTn20 = result.RTn20;
            target.RTp80 = result.RTp80;
            target.Cn20 = result.Cn20;
            target.Cp20 = result.Cp20;
            target.Cp80 = result.Cp80;
            target.beta = result.beta;
            target.A2 = result.A2;
            target.E = result.E;
            target.alpha = result.alpha;
            target.Woyp = result.Woyp;
            target.Woyn = result.Woyn;
            target.Wozp = result.Wozp;
            target.Wozn = result.Wozn;
            target.Wuyp = result.Wuyp;
            target.Wuyn = result.Wuyn;
            target.Wuzp = result.Wuzp;
            target.Wuzn = result.Wuzn;
            target.Depth = result.Depth;
        }

        internal void UpdateMaterialProperties(BpsUnifiedModel unifiedModel)
        {
            string insulationType = unifiedModel.ModelInput.FrameSystem.InsulationType;
            List<Section> intermediateSections = unifiedModel.ModelInput.Geometry.Sections.Where(x => x.SectionType == 2 || x.SectionType == 3).ToList();
            double beta;

            foreach (Section section in intermediateSections)
            {
                if (section.isCustomProfile)
                {
                    // update beta
                    switch (unifiedModel.ModelInput.FrameSystem.Alloys)
                    {
                        case "6060-T66 (150MPa)":
                            beta = 150;
                            break;

                        case "6063-T6 (160MPa) -DIN":
                            beta = 160;
                            break;

                        case "6063-T6 (170MPa) -US":
                            beta = 170;
                            break;

                        case "6061-T6 (240MPa)":
                            beta = 240;
                            break;

                        case "6065a-T6 (240MPa)":
                            beta = 240;
                            break;

                        default:
                            beta = 150;
                            break;
                    }
                    section.beta = beta;
                    continue;
                }
                int articleId = Convert.ToInt32(section.ArticleName);

                // update RSn20, RSp80, RTn20, RTp80, Cn20, Cp20, Cp80;
                InsulatingBar insluationBar = _db.InsulatingBar.Where(x => x.ArticleId == articleId).FirstOrDefault();
                string InsulationMaterial = String.Empty;
                switch (insulationType)
                {
                    case "Polythermid Coated Before":
                        InsulationMaterial = insluationBar.PTCoatedBefore;
                        break;

                    case "Polythermid Anodized After":
                        InsulationMaterial = insluationBar.PTAnodizedBefore;
                        break;

                    case "Polyamide Coated Before":
                        InsulationMaterial = insluationBar.PACoatedBefore;
                        break;

                    case "Polyamide Coated After":
                        InsulationMaterial = insluationBar.PACoatedAfter;
                        break;

                    case "Polyamide Anodized Before":
                        InsulationMaterial = insluationBar.PAAnodizedBefore;
                        break;

                    case "Polyamide Anodized After":
                        InsulationMaterial = insluationBar.PAAnodizedAfter;
                        break;

                    default:
                        InsulationMaterial = insluationBar.PAAnodizedAfter;
                        break;
                }
                unifiedModel.ModelInput.FrameSystem.InsulationMaterial = InsulationMaterial;

                int index = InsulationMaterial.IndexOf("*");
                string IBMaterial = index > 0 ? InsulationMaterial.Remove(index, 1) : InsulationMaterial;
                double RSn20, RSp80, RTn20, RTp80, Cn20, Cp20, Cp80;  // RSn, RSp in N/mm
                switch (IBMaterial)
                {
                    case "10":
                        switch (insulationType)
                        {
                            case "Polyamide Coated After":
                                RSn20 = 65.4; RSp80 = 43.2; RTn20 = 82.6; RTp80 = 80.5; Cn20 = 49; Cp20 = 27; Cp80 = 17;
                                break;

                            case "Polyamide Anodized After":
                                RSn20 = 106.4; RSp80 = 49.2; RTn20 = 85; RTp80 = 88.5; Cn20 = 36; Cp20 = 23; Cp80 = 19;
                                break;

                            default:
                                RSn20 = 106.4; RSp80 = 49.2; RTn20 = 85; RTp80 = 88.5; Cn20 = 36; Cp20 = 23; Cp80 = 19;
                                break;
                        }
                        break;

                    case "8":
                    default:
                        switch (insulationType)
                        {
                            case "Polythermid Coated Before":
                                RSn20 = 66.8; RSp80 = 59.6; RTn20 = 34; RTp80 = 40.7; Cn20 = 47; Cp20 = 45; Cp80 = 43;
                                break;

                            case "Polythermid Anodized After":
                                RSn20 = 64.8; RSp80 = 45.4; RTn20 = 48.4; RTp80 = 31.4; Cn20 = 45; Cp20 = 45; Cp80 = 41;
                                break;

                            case "Polyamide Coated Before":
                                RSn20 = 77.8; RSp80 = 48.5; RTn20 = 66.8; RTp80 = 52.6; Cn20 = 44; Cp20 = 40; Cp80 = 33;
                                break;

                            case "Polyamide Coated After":
                                RSn20 = 56.4; RSp80 = 49; RTn20 = 76.5; RTp80 = 47.5; Cn20 = 50; Cp20 = 42; Cp80 = 35;
                                break;

                            case "Polyamide Anodized Before":
                                RSn20 = 73.3; RSp80 = 29.7; RTn20 = 71.1; RTp80 = 45.8; Cn20 = 46; Cp20 = 41; Cp80 = 31;
                                break;

                            case "Polyamide Anodized After":
                                RSn20 = 87.3; RSp80 = 51.3; RTn20 = 64.4; RTp80 = 51; Cn20 = 78; Cp20 = 29; Cp80 = 46;
                                break;

                            default:
                                RSn20 = 87.3; RSp80 = 51.3; RTn20 = 64.4; RTp80 = 51; Cn20 = 78; Cp20 = 29; Cp80 = 46;
                                break;
                        }
                        break;
                }

                section.RSn20 = RSn20;
                section.RSp80 = RSp80;
                section.RTn20 = RTn20;
                section.RTp80 = RTp80;
                section.Cn20 = Cn20;
                section.Cp20 = Cp20;
                section.Cp80 = Cp80;

                // update beta
                switch (unifiedModel.ModelInput.FrameSystem.Alloys)
                {
                    case "6060-T66 (150MPa)":
                        beta = 150;
                        break;

                    case "6063-T6 (160MPa) -DIN":
                        beta = 160;
                        break;

                    case "6063-T6 (170MPa) -US":
                        beta = 170;
                        break;

                    case "6061-T6 (240MPa)":
                        beta = 240;
                        break;

                    case "6065a-T6 (240MPa)":
                        beta = 240;
                        break;

                    default:
                        beta = 150;
                        break;
                }
                section.beta = beta;

                //read other secion properties from database
                Article article = _db.Article.Where(x => x.Name == "article__" + section.ArticleName).FirstOrDefault();
                section.InsideW = (double)article.InsideDimension;
                section.OutsideW = (double)article.OutsideDimension;
                section.RightRebate = (double)article.RightRebate;
                section.LeftRebate = (double)article.LeftRebate;
                section.DistBetweenIsoBars = (double)article.DistBetweenIsoBars;
            }

            // set default values
            foreach (Section section in unifiedModel.ModelInput.Geometry.Sections)
            {
                if (section.A2 == 0) { section.A2 = 1.2; }
                if (section.E == 0) { section.E = 70000; }     // N/mm2
                if (section.alpha == 0) { section.alpha = 2.3E-5; }
            }
        }

        // read facade profile from dxf
        public void ReadFacadeSectionProperties(BpsUnifiedModel unifiedModel)
        {
            List<FacadeSection> facadeSections = unifiedModel.ModelInput.Geometry.FacadeSections;

            Dictionary<string, FacadeSection> articleNumFacadeSectionMap = new Dictionary<string, FacadeSection>();

            for (int i = 0; i < facadeSections.Count(); i++)
            {
                FacadeSection facadeSection = facadeSections[i];

                // Load result if already been calculated
                if (articleNumFacadeSectionMap.ContainsKey(facadeSection.ArticleName))
                {
                    LoadFacadeSectionResult(articleNumFacadeSectionMap[facadeSection.ArticleName], ref facadeSection);
                }
                else
                {
                    string dxfFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\dxf\", $"{facadeSection.ArticleName}.dxf");
                    if (File.Exists(dxfFilePath))
                    {
                        var DSP = new DXFFacadeSectionProperty();
                        DSP.ReadDXF(dxfFilePath, ref facadeSection);
                        articleNumFacadeSectionMap[facadeSection.ArticleName] = facadeSection;
                    }
                }
            }
            // update material here
            UpdateFacadeMaterialProperties(unifiedModel);
        }

        // read facade profile from DB
        public void ReadFacadeSectionPropertiesFromDB(BpsUnifiedModel unifiedModel)
        {
            List<FacadeSection> facadeSections = unifiedModel.ModelInput.Geometry.FacadeSections;

            Dictionary<string, FacadeSection> articleNumFacadeSectionMap = new Dictionary<string, FacadeSection>();

            for (int i = 0; i < facadeSections.Count(); i++)
            {
                FacadeSection facadeSection = facadeSections[i];

                if (facadeSection.ArticleName == "100000") continue; // dummy article

                // Load result if already been calculated
                if (articleNumFacadeSectionMap.ContainsKey(facadeSection.ArticleName))
                {
                    LoadFacadeSectionResult(articleNumFacadeSectionMap[facadeSection.ArticleName], ref facadeSection);
                }
                else
                {
                    FacadeArticleSectionalProperty dbFacadeArticleSectionalProperty = _db.FacadeArticleSectionalProperty.Where(x => x.ArticleName == facadeSection.ArticleName).SingleOrDefault();
                    if (dbFacadeArticleSectionalProperty == null)
                    {
                        throw new InvalidDataException();
                    }
                    else
                    {
                        LoadFacadeSectionPropertyFromDB(dbFacadeArticleSectionalProperty, ref facadeSection);
                        articleNumFacadeSectionMap[facadeSection.ArticleName] = facadeSection;
                    }
                }
            }
            // update material here
            UpdateFacadeMaterialProperties(unifiedModel);
        }

        internal void LoadFacadeSectionResult(FacadeSection result, ref FacadeSection target)
        {
            target.OutsideW = result.OutsideW;
            target.BTDepth = result.BTDepth;
            target.Width = result.Width;
            target.Zo = result.Zo;
            target.Zu = result.Zu;
            target.Zl = result.Zl;
            target.Zr = result.Zr;
            target.A = result.A;
            target.Material = result.Material;
            target.beta = result.beta;
            target.Weight = result.Weight;
            target.Iyy = result.Iyy;
            target.Izz = result.Izz;
            target.Wyy = result.Wyy;
            target.Wzz = result.Wzz;
            target.Asy = result.Asy;
            target.Asz = result.Asz;
            target.J = result.J;
            target.E = result.E;
            target.G = result.G;
            target.EA = result.EA;
            target.GAsy = result.GAsy;
            target.GAsz = result.GAsz;
            target.EIy = result.EIy;
            target.EIz = result.EIz;
            target.GJ = result.GJ;
            target.Ys = result.Ys;
            target.Zs = result.Zs;
            target.Ry = result.Ry;
            target.Rz = result.Rz;
            target.Wyp = result.Wyp;
            target.Wyn = result.Wyn;
            target.Wzp = result.Wzp;
            target.Wzn = result.Wzn;
            target.Cw = result.Cw;
            target.Beta_torsion = result.Beta_torsion;
            target.Zy = result.Zy;
            target.Zz = result.Zz;
            target.Depth = result.BTDepth;
        }

        internal void LoadFacadeSectionPropertyFromDB(FacadeArticleSectionalProperty result, ref FacadeSection target)
        {
            target.OutsideW = result.OutsideW;
            target.BTDepth = result.BTDepth;
            target.Width = result.Width;
            target.Zo = result.Zo;
            target.Zu = result.Zu;
            target.Zl = result.Zl;
            target.Zr = result.Zr;
            target.A = result.A;
            target.Material = result.Material;
            target.beta = result.beta;
            target.Weight = result.Weight;
            target.Iyy = result.Iyy;
            target.Izz = result.Izz;
            target.Wyy = result.Wyy;
            target.Wzz = result.Wzz;
            target.Asy = result.Asy;
            target.Asz = result.Asz;
            target.J = result.J;
            target.E = result.E;
            target.G = result.G;
            target.EA = result.EA;
            target.GAsy = result.GAsy;
            target.GAsz = result.GAsz;
            target.EIy = result.EIy;
            target.EIz = result.EIz;
            target.GJ = result.GJ;
            target.Ys = result.Ys;
            target.Zs = result.Zs;
            target.Ry = result.Ry;
            target.Rz = result.Rz;
            target.Wyp = result.Wyp;
            target.Wyn = result.Wyn;
            target.Wzp = result.Wzp;
            target.Wzn = result.Wzn;
            target.Cw = result.Cw;
            target.Beta_torsion = result.Beta_torsion;
            target.Zy = result.Zy;
            target.Zz = result.Zz;
            target.Depth = result.BTDepth;
        }

        internal void UpdateFacadeMaterialProperties(BpsUnifiedModel unifiedModel)
        {
            foreach (FacadeSection facadeSection in unifiedModel.ModelInput.Geometry.FacadeSections)
            {
                // update beta
                double beta;
                switch (unifiedModel.ModelInput.FrameSystem.Alloys)
                {
                    case "6060-T66 (150MPa)":
                        beta = 150;
                        break;

                    case "6063-T6 (160MPa) -DIN":
                        beta = 160;
                        break;

                    case "6063-T6 (170MPa) -US":
                        beta = 170;
                        break;

                    case "6061-T6 (240MPa)":
                        beta = 240;
                        break;

                    case "6065a-T6 (240MPa)":
                        beta = 240;
                        break;

                    default:
                        beta = 150;
                        break;
                }
                facadeSection.beta = beta;
            }

            // set default values
            foreach (FacadeSection facadeSection in unifiedModel.ModelInput.Geometry.FacadeSections)
            {
                if (facadeSection.Material == "Aluminum")
                {
                    facadeSection.E = 70000;     // N/mm2
                    facadeSection.G = facadeSection.E / 2.66;    // N/mm2 poisson's ratio 0.33
                }
                else if (facadeSection.Material == "Steel")
                {
                    facadeSection.E = 210000;      // N/mm2
                    facadeSection.G = facadeSection.E / 2.6;      // N/mm2 poisson's ratio 0.3
                }

                facadeSection.Asy = facadeSection.A;
                facadeSection.Asz = facadeSection.A;
                facadeSection.EA = facadeSection.E * facadeSection.A;
                facadeSection.GAsy = facadeSection.G * facadeSection.Asy;
                facadeSection.GAsz = facadeSection.G * facadeSection.Asz;
                facadeSection.EIy = facadeSection.E * facadeSection.Iyy;
                facadeSection.EIz = facadeSection.E * facadeSection.Izz;
                facadeSection.GJ = facadeSection.G * facadeSection.J;
            }
        }
    }
}