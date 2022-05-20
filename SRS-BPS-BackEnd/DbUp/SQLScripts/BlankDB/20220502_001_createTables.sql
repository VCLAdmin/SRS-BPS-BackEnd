CREATE DATABASE  IF NOT EXISTS `vcldesigndb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `vcldesigndb`;
-- MySQL dump 10.13  Distrib 8.0.28, for Win64 (x86_64)
--
-- Host: localhost    Database: vcldesigndb
-- ------------------------------------------------------
-- Server version	8.0.28

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `accessrole`
--

DROP TABLE IF EXISTS `accessrole`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `accessrole` (
  `AccessRoleId` int NOT NULL,
  `AccessRoleName` varchar(45) NOT NULL,
  PRIMARY KEY (`AccessRoleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;





--
-- Table structure for table `acousticreport`
--

DROP TABLE IF EXISTS `acousticreport`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `acousticreport` (
  `AcousticReportId` bigint NOT NULL AUTO_INCREMENT,
  `FileName` varchar(45) DEFAULT NULL,
  `ProductName` varchar(45) NOT NULL,
  `ProductCode` varchar(45) NOT NULL,
  `ProductType` varchar(45) DEFAULT NULL,
  `OpeningType` varchar(45) DEFAULT NULL,
  `GlassLite1` varchar(45) NOT NULL,
  `GlassAirSpaceOne` varchar(45) NOT NULL,
  `GlassLite2` varchar(45) NOT NULL,
  `GlassAirSpaceTwo` varchar(45) DEFAULT NULL,
  `GlassLite3` varchar(45) DEFAULT NULL,
  `STC` int DEFAULT NULL,
  `OITC` int DEFAULT NULL,
  `Rw` int DEFAULT NULL,
  `C` int DEFAULT NULL,
  `Ctr` int DEFAULT NULL,
  `TransmissionLoss50` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss63` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss80` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss100` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss125` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss160` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss200` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss250` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss315` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss400` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss500` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss630` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss800` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss1000` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss1250` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss1600` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss2000` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss2500` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss3150` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss4000` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss5000` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss6300` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss8000` decimal(4,1) DEFAULT NULL,
  `TransmissionLoss10000` decimal(4,1) DEFAULT NULL,
  PRIMARY KEY (`AcousticReportId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;




DROP TABLE IF EXISTS `address`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `address` (
  `AddressId` int NOT NULL AUTO_INCREMENT,
  `AddressExternalId` char(36) NOT NULL,
  `Line1` text NOT NULL,
  `Line2` text,
  `Country` varchar(90) DEFAULT NULL,
  `State` varchar(90) DEFAULT NULL,
  `County` varchar(90) DEFAULT NULL,
  `City` varchar(90) NOT NULL,
  `PostalCode` varchar(45) NOT NULL,
  `AdditionalDetails` text COMMENT 'This will be used to describe the Address Type - Work, HQ, Remote Office, Temporary Office etc.. ',
  `CreatedBy` int DEFAULT NULL,
  `CreatedOn` datetime DEFAULT NULL,
  `ModifiedBy` int DEFAULT NULL,
  `ModifiedOn` datetime DEFAULT NULL,
  `AddressType` varchar(45) DEFAULT NULL,
  `Active` tinyint(1) DEFAULT '1',
  `ProjectId` int DEFAULT NULL,
  `Latitude` decimal(12,9) DEFAULT NULL,
  `Longitude` decimal(12,9) DEFAULT NULL,
  PRIMARY KEY (`AddressId`),
  KEY `Address_CreatedBy_idx` (`CreatedBy`),
  KEY `Address_ModifiedBy_idx` (`ModifiedBy`),
  CONSTRAINT `Address_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `user` (`UserId`),
  CONSTRAINT `Address_ModifiedBy` FOREIGN KEY (`ModifiedBy`) REFERENCES `user` (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=421 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;




--
-- Table structure for table `adsarticle`
--

DROP TABLE IF EXISTS `adsarticle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `adsarticle` (
  `ArticleSetID` int NOT NULL,
  `ArticleName` text,
  `System` text,
  `Type` text,
  `InsideW` int DEFAULT NULL,
  `OutsideW` int DEFAULT NULL,
  PRIMARY KEY (`ArticleSetID`),
  UNIQUE KEY `ArticleSetID_UNIQUE` (`ArticleSetID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;





--
-- Table structure for table `article`
--

DROP TABLE IF EXISTS `article`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `article` (
  `ArticleId` int NOT NULL AUTO_INCREMENT,
  `ArticleGuid` char(36) DEFAULT NULL,
  `Name` varchar(45) NOT NULL,
  `Unit` varchar(45) NOT NULL,
  `ArticleTypeId` int NOT NULL,
  `CrossSectionUrl` varchar(45) DEFAULT NULL,
  `Description` varchar(45) NOT NULL,
  `InsideDimension` double DEFAULT NULL,
  `OutsideDimension` double DEFAULT NULL,
  `Dimension` double DEFAULT NULL,
  `OffsetReference` varchar(45) DEFAULT NULL,
  `LeftRebate` double DEFAULT NULL,
  `RightRebate` double DEFAULT NULL,
  `DistBetweenIsoBars` double DEFAULT NULL,
  `Depth` double DEFAULT NULL,
  PRIMARY KEY (`ArticleId`),
  KEY `Article_ArticleTypeId` (`ArticleTypeId`),
  CONSTRAINT `Article_ArticleTypeId` FOREIGN KEY (`ArticleTypeId`) REFERENCES `articletype` (`ArticleTypeId`)
) ENGINE=InnoDB AUTO_INCREMENT=238 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;




DROP TABLE IF EXISTS `articlesectionalproperty`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `articlesectionalproperty` (
  `ArticleName` varchar(45) NOT NULL,
  `InsideW` double NOT NULL,
  `OutsideW` double NOT NULL,
  `LeftRebate` double NOT NULL,
  `RightRebate` double NOT NULL,
  `DistBetweenIsoBars` double NOT NULL,
  `d` double NOT NULL,
  `Weight` double NOT NULL,
  `Ao` double NOT NULL,
  `Au` double NOT NULL,
  `Io` double NOT NULL,
  `Iu` double NOT NULL,
  `Ioyy` double NOT NULL,
  `Iuyy` double NOT NULL,
  `Zoo` double NOT NULL,
  `Zou` double NOT NULL,
  `Zol` double NOT NULL,
  `Zor` double NOT NULL,
  `Zuo` double NOT NULL,
  `Zuu` double NOT NULL,
  `Zul` double NOT NULL,
  `Zur` double NOT NULL,
  `RSn20` double NOT NULL,
  `RSp80` double NOT NULL,
  `RTn20` double NOT NULL,
  `RTp80` double NOT NULL,
  `Cn20` double NOT NULL,
  `Cp20` double NOT NULL,
  `Cp80` double NOT NULL,
  `beta` double NOT NULL,
  `A2` double NOT NULL,
  `E` double NOT NULL,
  `alpha` double NOT NULL,
  `Woyp` double NOT NULL,
  `Woyn` double NOT NULL,
  `Wozp` double NOT NULL,
  `Wozn` double NOT NULL,
  `Wuyp` double NOT NULL,
  `Wuyn` double NOT NULL,
  `Wuzp` double NOT NULL,
  `Wuzn` double NOT NULL,
  `Depth` double NOT NULL,
  PRIMARY KEY (`ArticleName`),
  UNIQUE KEY `ArticleName_UNIQUE` (`ArticleName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


--
-- Table structure for table `articletype`
--

DROP TABLE IF EXISTS `articletype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `articletype` (
  `ArticleTypeId` int NOT NULL AUTO_INCREMENT,
  `ArticleTypeGuid` char(36) DEFAULT NULL,
  `PrettyName` varchar(45) NOT NULL,
  `Name` varchar(45) NOT NULL,
  PRIMARY KEY (`ArticleTypeId`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `asearticle`
--

DROP TABLE IF EXISTS `asearticle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `asearticle` (
  `ArticleSetID` int NOT NULL,
  `ArticleName` text NOT NULL,
  `System` text NOT NULL,
  `SRS` int DEFAULT NULL,
  `Category` text,
  `ProfileType` text,
  `InsideW` double DEFAULT NULL,
  `OutsideW` double DEFAULT NULL,
  `Depth` double DEFAULT NULL,
  `IsolatorType` text,
  `ExtrusionLength` double DEFAULT NULL,
  PRIMARY KEY (`ArticleSetID`),
  UNIQUE KEY `ArticleSetID_UNIQUE` (`ArticleSetID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `aspnetroles`
--

DROP TABLE IF EXISTS `aspnetroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroles` (
  `Id` varchar(128) NOT NULL,
  `Name` varchar(256) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `aspnetuserclaims`
--

DROP TABLE IF EXISTS `aspnetuserclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` varchar(128) NOT NULL,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id` (`Id`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `ApplicationUser_Claims` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `aspnetuserlogins`
--

DROP TABLE IF EXISTS `aspnetuserlogins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserlogins` (
  `LoginProvider` varchar(128) NOT NULL,
  `ProviderKey` varchar(128) NOT NULL,
  `UserId` varchar(128) NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`,`UserId`),
  KEY `ApplicationUser_Logins` (`UserId`),
  CONSTRAINT `ApplicationUser_Logins` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


--
-- Table structure for table `aspnetuserroles`
--

DROP TABLE IF EXISTS `aspnetuserroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserroles` (
  `UserId` varchar(128) NOT NULL,
  `RoleId` varchar(128) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IdentityRole_Users` (`RoleId`),
  CONSTRAINT `ApplicationUser_Roles` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `IdentityRole_Users` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `aspnetusers`
--

DROP TABLE IF EXISTS `aspnetusers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusers` (
  `Id` varchar(128) NOT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext,
  `SecurityStamp` longtext,
  `PhoneNumber` longtext,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEndDateUtc` datetime DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int NOT NULL,
  `UserName` varchar(256) NOT NULL,
  `IsDefaultPasswordUpdated` tinyint(1) NOT NULL DEFAULT '0',
  `AgreementConfirmed` tinyint(1) NOT NULL DEFAULT '0',
  `AgreementUpdatedOn` datetime DEFAULT NULL,
  `WebsiteTracking` bit(1) NOT NULL DEFAULT b'1',
  `ReleaseVersion` bit(1) DEFAULT NULL,
  `EncryptedPassword` longtext,
  `FirstLoggedOn` datetime DEFAULT NULL,
  `LastLoggedOn` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bpsboundarycondition`
--

DROP TABLE IF EXISTS `bpsboundarycondition`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bpsboundarycondition` (
  `BoundaryConditionId` int NOT NULL AUTO_INCREMENT,
  `BoundaryConditionGuid` char(36) NOT NULL,
  `ProblemId` int NOT NULL,
  `BoundaryConditionNode` blob NOT NULL,
  PRIMARY KEY (`BoundaryConditionId`),
  UNIQUE KEY `BoundaryConditionGuid_UNIQUE` (`BoundaryConditionGuid`),
  KEY `FK_BpsBoundaryCondition` (`ProblemId`),
  CONSTRAINT `FK_BpsBoundaryCondition` FOREIGN KEY (`ProblemId`) REFERENCES `bpsproblem` (`ProblemId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bpsproblem`
--

DROP TABLE IF EXISTS `bpsproblem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bpsproblem` (
  `ProblemId` int NOT NULL AUTO_INCREMENT,
  `ProblemGuid` char(36) NOT NULL,
  `ProjectId` int NOT NULL,
  `CreatedOn` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE CURRENT_TIMESTAMP,
  `ProblemName` varchar(45) NOT NULL,
  `SystemModel` longtext,
  `SystemImage` blob,
  `SystemName` varchar(45) DEFAULT NULL,
  `SightlineArticleNumber` varchar(45) DEFAULT NULL,
  `IntermediateArticleNumber` varchar(45) DEFAULT NULL,
  `GlassConfigurations` text,
  `WallType` varchar(45) DEFAULT NULL,
  `WallHeight` double DEFAULT NULL,
  `WallWidth` double DEFAULT NULL,
  `RoomArea` double DEFAULT NULL,
  `WindLoad` double DEFAULT NULL,
  `EngineeringStandard` varchar(45) DEFAULT NULL,
  `BuildingLength` double DEFAULT NULL,
  `BuildingWidth` double DEFAULT NULL,
  `BuildingRiskCategory` varchar(45) DEFAULT NULL,
  `WindSpeed` double DEFAULT NULL,
  `ExposureCategory` varchar(45) DEFAULT NULL,
  `WindowWidth` double DEFAULT NULL,
  `WindowHeight` double DEFAULT NULL,
  `WindowElevation` double DEFAULT NULL,
  `RelativeHumidity` double DEFAULT NULL,
  `PhysicsTypeAcoustic` tinyint DEFAULT NULL,
  `PhysicsTypeStructure` tinyint DEFAULT NULL,
  `PhysicsTypeThermal` tinyint DEFAULT NULL,
  `ProductTypeId` int DEFAULT NULL,
  `OperabilityConfigurations` text,
  `WindowZone` int DEFAULT NULL,
  `BuildingHeight` double DEFAULT NULL,
  `TerrainCategory` int DEFAULT NULL,
  `VentFrameArticleNumber` varchar(45) DEFAULT NULL,
  `CustomArticles` text,
  `WindZone` int DEFAULT NULL,
  `GlazingGasketCombination` varchar(45) DEFAULT NULL,
  `Alloys` varchar(45) DEFAULT NULL,
  `InsulatingBar` varchar(45) DEFAULT NULL,
  `PermissibleDeflection` int DEFAULT NULL,
  `PermissibleVerticalDeflection` int DEFAULT NULL,
  `UnifiedObjectModel` mediumtext,
  `AcousticReportUrl` text,
  `StructuralReportUrl` text,
  `ThermalReportUrl` text,
  `AcousticResults` mediumtext,
  `StructuralResults` mediumtext,
  `ThermalResults` mediumtext,
  PRIMARY KEY (`ProblemId`),
  UNIQUE KEY `ProblemGuid_UNIQUE` (`ProblemGuid`),
  KEY `BpsProblem_ProductTypeId` (`ProductTypeId`),
  KEY `BpsProblem_ProjectId` (`ProjectId`),
  CONSTRAINT `BpsProblem_ProductTypeId` FOREIGN KEY (`ProductTypeId`) REFERENCES `producttype` (`ProductTypeId`),
  CONSTRAINT `BpsProblem_ProjectId` FOREIGN KEY (`ProjectId`) REFERENCES `bpsproject` (`ProjectId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bpsproject`
--

DROP TABLE IF EXISTS `bpsproject`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bpsproject` (
  `ProjectId` int NOT NULL AUTO_INCREMENT,
  `ProjectGuid` char(36) NOT NULL,
  `UserId` int NOT NULL,
  `ProjectName` varchar(45) DEFAULT NULL,
  `ProjectLocation` varchar(255) DEFAULT NULL,
  `CreatedOn` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` timestamp NULL DEFAULT NULL,
  `AddressId` int DEFAULT NULL,
  PRIMARY KEY (`ProjectId`),
  UNIQUE KEY `ProjectGuid_UNIQUE` (`ProjectGuid`),
  KEY `BpsProject_UserId` (`UserId`),
  KEY `BpsProject_AddressId_idx` (`AddressId`),
  CONSTRAINT `BpsProject_AddressId` FOREIGN KEY (`AddressId`) REFERENCES `address` (`AddressId`),
  CONSTRAINT `BpsProject_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=1650 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bpsunifiedproblem`
--

DROP TABLE IF EXISTS `bpsunifiedproblem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bpsunifiedproblem` (
  `ProblemId` int NOT NULL AUTO_INCREMENT,
  `ProblemGuid` char(36) NOT NULL,
  `ProblemName` varchar(45) NOT NULL,
  `ProjectId` int NOT NULL,
  `CreatedOn` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UnifiedModel` mediumtext NOT NULL,
  PRIMARY KEY (`ProblemId`),
  UNIQUE KEY `ProblemGuid_UNIQUE` (`ProblemGuid`)
) ENGINE=InnoDB AUTO_INCREMENT=2094 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `customarticle`
--

DROP TABLE IF EXISTS `customarticle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customarticle` (
  `CustomArticleId` int NOT NULL AUTO_INCREMENT,
  `CustomArticleGuid` char(36) DEFAULT NULL,
  `ArticleId` int NOT NULL,
  `Category` varchar(45) NOT NULL,
  `Spacer` varchar(45) NOT NULL,
  `Element_level_1` varchar(45) DEFAULT NULL,
  `Element_type_1` varchar(45) DEFAULT NULL,
  `Element_size_1` varchar(45) DEFAULT NULL,
  `Element_interlayer_1` varchar(45) DEFAULT NULL,
  `Element_lam_type_1` varchar(45) DEFAULT NULL,
  `Element_lam_size_1` varchar(45) DEFAULT NULL,
  `Element_level_2` varchar(45) DEFAULT NULL,
  `Element_type_2` varchar(45) DEFAULT NULL,
  `Element_size_2` varchar(45) DEFAULT NULL,
  `Element_interlayer_2` varchar(45) DEFAULT NULL,
  `Element_lam_type_2` varchar(45) DEFAULT NULL,
  `Element_lam_size_2` varchar(45) DEFAULT NULL,
  `Element_level_3` varchar(45) DEFAULT NULL,
  `Element_type_3` varchar(45) DEFAULT NULL,
  `Element_size_3` varchar(45) DEFAULT NULL,
  `Element_interlayer_3` varchar(45) DEFAULT NULL,
  `UValue` varchar(45) DEFAULT NULL,
  `GlassRW` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`CustomArticleId`),
  KEY `CustomArticle_ArticleId_idx` (`ArticleId`),
  CONSTRAINT `CustomArticle_ArticleId` FOREIGN KEY (`ArticleId`) REFERENCES `article` (`ArticleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dealer`
--

DROP TABLE IF EXISTS `dealer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dealer` (
  `DealerId` int NOT NULL AUTO_INCREMENT,
  `DealerExternalId` char(36) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `PrimaryContactName` varchar(45) DEFAULT NULL,
  `AddressId` int DEFAULT NULL,
  `PrimaryContactEmail` varchar(255) DEFAULT NULL,
  `PrimaryContactPhone` varchar(45) DEFAULT NULL,
  `CreditLine` double NOT NULL,
  `DefaultSalesTaxRate` double DEFAULT NULL,
  `AWSFabricatorId` int NOT NULL,
  `ADSFabricatorId` int NOT NULL,
  `ASSFabricatorId` int NOT NULL,
  PRIMARY KEY (`DealerId`),
  KEY `Dealer_Address_idx` (`AddressId`),
  KEY `Dealer_AWSFabricator_idx` (`AWSFabricatorId`),
  KEY `Dealer_ADSFabricator_idx` (`ADSFabricatorId`),
  KEY `Dealer_ASSFabricator_idx` (`ASSFabricatorId`),
  CONSTRAINT `Dealer_Address` FOREIGN KEY (`AddressId`) REFERENCES `address` (`AddressId`),
  CONSTRAINT `Dealer_ADSFabricator` FOREIGN KEY (`ADSFabricatorId`) REFERENCES `fabricator` (`FabricatorId`),
  CONSTRAINT `Dealer_ASSFabricator` FOREIGN KEY (`ASSFabricatorId`) REFERENCES `fabricator` (`FabricatorId`),
  CONSTRAINT `Dealer_AWSFabricator` FOREIGN KEY (`AWSFabricatorId`) REFERENCES `fabricator` (`FabricatorId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dealer_user`
--

DROP TABLE IF EXISTS `dealer_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dealer_user` (
  `DealerId` int NOT NULL,
  `UserId` int NOT NULL,
  PRIMARY KEY (`DealerId`,`UserId`),
  KEY `UserId_FK_idx` (`UserId`),
  CONSTRAINT `DealerId_FK` FOREIGN KEY (`DealerId`) REFERENCES `dealer` (`DealerId`),
  CONSTRAINT `UserId_FK` FOREIGN KEY (`UserId`) REFERENCES `user` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `doorhandlehinge`
--

DROP TABLE IF EXISTS `doorhandlehinge`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `doorhandlehinge` (
  `ArticleSetID` int NOT NULL,
  `ArticleName` text,
  `Color` text,
  `Color Code` text,
  `Type` text,
  `Description` text,
  PRIMARY KEY (`ArticleSetID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `externalclient`
--

DROP TABLE IF EXISTS `externalclient`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `externalclient` (
  `ClientId` bigint NOT NULL AUTO_INCREMENT,
  `ClientExternalId` char(36) NOT NULL,
  `ClientName` varchar(200) NOT NULL,
  `ClientSecret` varchar(250) NOT NULL,
  `ClientUri` varchar(2000) DEFAULT NULL,
  `CreatedBy` bigint NOT NULL,
  `CreatedOn` datetime NOT NULL,
  `ModifiedBy` bigint DEFAULT NULL,
  `ModifiedOn` datetime DEFAULT NULL,
  PRIMARY KEY (`ClientId`),
  UNIQUE KEY `ClientSecret_UNIQUE` (`ClientSecret`),
  UNIQUE KEY `ClientId_UNIQUE` (`ClientExternalId`),
  UNIQUE KEY `ClientName_UNIQUE` (`ClientName`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fabricator`
--

DROP TABLE IF EXISTS `fabricator`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `fabricator` (
  `FabricatorId` int NOT NULL AUTO_INCREMENT,
  `FabricatorExternalId` char(36) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `PrimaryContactName` varchar(45) DEFAULT NULL,
  `AddressId` int DEFAULT NULL,
  `PrimaryContactEmail` varchar(255) DEFAULT NULL,
  `PrimaryContactPhone` varchar(45) DEFAULT NULL,
  `SupportsAWS` tinyint NOT NULL DEFAULT '0',
  `SupportsADS` tinyint NOT NULL DEFAULT '0',
  `SupportsASS` tinyint NOT NULL DEFAULT '0',
  PRIMARY KEY (`FabricatorId`),
  KEY `Fabricator_Address_idx` (`AddressId`),
  CONSTRAINT `Fabricator_Address` FOREIGN KEY (`AddressId`) REFERENCES `address` (`AddressId`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fabricator_user`
--

DROP TABLE IF EXISTS `fabricator_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `fabricator_user` (
  `FabricatorId` int NOT NULL,
  `UserId` int NOT NULL,
  PRIMARY KEY (`FabricatorId`,`UserId`),
  KEY `Usr_UK_idx` (`UserId`),
  CONSTRAINT `Fab_FK` FOREIGN KEY (`FabricatorId`) REFERENCES `fabricator` (`FabricatorId`),
  CONSTRAINT `Usr_UK` FOREIGN KEY (`UserId`) REFERENCES `user` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `facadearticle`
--

DROP TABLE IF EXISTS `facadearticle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `facadearticle` (
  `ArticleSetID` int NOT NULL,
  `System` text NOT NULL,
  `Mullion` text,
  `Mullion Depth` int DEFAULT NULL,
  `Mullion Reinforcement Type1` text,
  `Mullion Reinforcement Type1 Material` text,
  `Mullion Reinforcement Type2` text,
  `Mullion Reinforcement Type2 Material` text,
  `Transom` text,
  `Transom Depth` int DEFAULT NULL,
  `Level 2 Transom` text,
  `Level 2 Transom Depth` int DEFAULT NULL,
  `Transom Reinforcement` text,
  `Transom Reinforcement Material` text,
  PRIMARY KEY (`ArticleSetID`),
  UNIQUE KEY `ArticleSetID_UNIQUE` (`ArticleSetID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `facadearticlesectionalproperty`
--

DROP TABLE IF EXISTS `facadearticlesectionalproperty`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `facadearticlesectionalproperty` (
  `ArticleName` varchar(45) NOT NULL,
  `OutsideW` double NOT NULL,
  `BTDepth` double NOT NULL,
  `Width` double NOT NULL,
  `Zo` double NOT NULL,
  `Zu` double NOT NULL,
  `Zl` double NOT NULL,
  `Zr` double NOT NULL,
  `A` double NOT NULL,
  `Material` varchar(45) NOT NULL,
  `beta` double NOT NULL,
  `Weight` double NOT NULL,
  `Iyy` double NOT NULL,
  `Izz` double NOT NULL,
  `Wyy` double NOT NULL,
  `Wzz` double NOT NULL,
  `Asy` double NOT NULL,
  `Asz` double NOT NULL,
  `J` double NOT NULL,
  `E` double NOT NULL,
  `G` double NOT NULL,
  `EA` double NOT NULL,
  `GAsy` double NOT NULL,
  `GAsz` double NOT NULL,
  `EIy` double NOT NULL,
  `EIz` double NOT NULL,
  `GJ` double NOT NULL,
  `Ys` double NOT NULL,
  `Zs` double NOT NULL,
  `Ry` double NOT NULL,
  `Rz` double NOT NULL,
  `Wyp` double NOT NULL,
  `Wyn` double NOT NULL,
  `Wzp` double NOT NULL,
  `Wzn` double NOT NULL,
  `Cw` double NOT NULL,
  `Beta_torsion` double NOT NULL,
  `Zy` double NOT NULL,
  `Zz` double NOT NULL,
  `Depth` double NOT NULL,
  PRIMARY KEY (`ArticleName`),
  UNIQUE KEY `ArticleName_UNIQUE` (`ArticleName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `facadeinsertunit`
--

DROP TABLE IF EXISTS `facadeinsertunit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `facadeinsertunit` (
  `FacadeInsertUnitId` int NOT NULL AUTO_INCREMENT,
  `FacadeInsertUnitGuid` char(36) DEFAULT NULL,
  `System` varchar(225) NOT NULL,
  `OpeningType` varchar(45) NOT NULL,
  `Material` varchar(45) NOT NULL,
  `GlassThicknessMin` int NOT NULL,
  `GlassThicknessMax` int NOT NULL,
  `Uf` double NOT NULL,
  PRIMARY KEY (`FacadeInsertUnitId`)
) ENGINE=InnoDB AUTO_INCREMENT=424 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `facadeprofile`
--

DROP TABLE IF EXISTS `facadeprofile`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `facadeprofile` (
  `FacadeProfileId` int NOT NULL AUTO_INCREMENT,
  `FacadeProfileGuid` char(36) DEFAULT NULL,
  `System` varchar(225) NOT NULL,
  `InsulationZone` varchar(45) DEFAULT NULL,
  `ProfileType` varchar(45) NOT NULL,
  `GlassThicknessMin` int NOT NULL,
  `GlassThicknessMax` int NOT NULL,
  `FacadeProfileTable_k` double NOT NULL,
  `FacadeProfileTable_l` double NOT NULL,
  PRIMARY KEY (`FacadeProfileId`)
) ENGINE=InnoDB AUTO_INCREMENT=203 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `facadespacer`
--

DROP TABLE IF EXISTS `facadespacer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `facadespacer` (
  `FacadeSpacerId` int NOT NULL AUTO_INCREMENT,
  `FacadeSpacerGuid` char(36) DEFAULT NULL,
  `System` varchar(225) NOT NULL,
  `InsulationZone` varchar(45) NOT NULL,
  `GlazingLayer` int NOT NULL,
  `Depth` int NOT NULL,
  `FacadeSpacer_1` double NOT NULL,
  `FacadeSpacer_2` double NOT NULL,
  `FacadeSpacer_3` double NOT NULL,
  `FacadeSpacer_83` double NOT NULL,
  `FacadeSpacer_51` double NOT NULL,
  `FacadeSpacer_22` double NOT NULL,
  `FacadeSpacer_31` double NOT NULL,
  `FacadeSpacer_71` double NOT NULL,
  `FacadeSpacer_52` double NOT NULL,
  `FacadeSpacer_85` double NOT NULL,
  `FacadeSpacer_92` double NOT NULL,
  `FacadeSpacer_101` double NOT NULL,
  `FacadeSpacer_93` double NOT NULL,
  `FacadeSpacer_21` double NOT NULL,
  `FacadeSpacer_111` double NOT NULL,
  `FacadeSpacer_61` double NOT NULL,
  `FacadeSpacer_72` double NOT NULL,
  `FacadeSpacer_81` double NOT NULL,
  `FacadeSpacer_84` double NOT NULL,
  `FacadeSpacer_11` double NOT NULL,
  `FacadeSpacer_91` double NOT NULL,
  `FacadeSpacer_23` double NOT NULL,
  `FacadeSpacer_41` double NOT NULL,
  `FacadeSpacer_82` double NOT NULL,
  PRIMARY KEY (`FacadeSpacerId`)
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `feature`
--

DROP TABLE IF EXISTS `feature`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `feature` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FeatureGuid` char(36) NOT NULL,
  `Feature` varchar(256) DEFAULT NULL,
  `ParentId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `FeatureGuid_UNIQUE` (`FeatureGuid`),
  KEY `ParentId_idx` (`ParentId`),
  CONSTRAINT `ParentId` FOREIGN KEY (`ParentId`) REFERENCES `feature` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=123 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `feature_permission_role`
--

DROP TABLE IF EXISTS `feature_permission_role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `feature_permission_role` (
  `FeatureId` int NOT NULL,
  `PermissionRoleId` int NOT NULL,
  PRIMARY KEY (`FeatureId`,`PermissionRoleId`),
  KEY `PermissionRoleId_idx` (`PermissionRoleId`),
  CONSTRAINT `FeatureId` FOREIGN KEY (`FeatureId`) REFERENCES `feature` (`Id`),
  CONSTRAINT `PermissionRoleId` FOREIGN KEY (`PermissionRoleId`) REFERENCES `permission_role` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `financial`
--

DROP TABLE IF EXISTS `financial`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `financial` (
  `FinancialId` int NOT NULL AUTO_INCREMENT,
  `FinancialExternalId` char(36) NOT NULL,
  `DealerId` int NOT NULL,
  `LineOfCredit` double NOT NULL,
  `OrdersToDate` double NOT NULL,
  `PaidToDate` double NOT NULL,
  `CurrentBalance` double NOT NULL,
  `CreatedBy` int NOT NULL,
  `CreatedOn` date NOT NULL,
  `ModifiedBy` int DEFAULT NULL,
  `ModifiedOn` date DEFAULT NULL,
  PRIMARY KEY (`FinancialId`),
  KEY `Financial_Dealer_idx` (`DealerId`),
  KEY `Financial_CreatedBy_idx` (`CreatedBy`),
  KEY `Financial_ModifiedBy_idx` (`ModifiedBy`),
  CONSTRAINT `Financial_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `user` (`UserId`),
  CONSTRAINT `Financial_Dealer` FOREIGN KEY (`DealerId`) REFERENCES `dealer` (`DealerId`),
  CONSTRAINT `Financial_ModifiedBy` FOREIGN KEY (`ModifiedBy`) REFERENCES `user` (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `glassbps`
--

DROP TABLE IF EXISTS `glassbps`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `glassbps` (
  `GlassTypeID` int NOT NULL,
  `GlassType` text,
  `Composition` text,
  `Type` text,
  `Total Thickness` double DEFAULT NULL,
  `U-value` double DEFAULT NULL,
  `Rw` int DEFAULT NULL,
  `Spacer` int DEFAULT NULL,
  PRIMARY KEY (`GlassTypeID`),
  UNIQUE KEY `GlassTypeID_UNIQUE` (`GlassTypeID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `glasssrs`
--

DROP TABLE IF EXISTS `glasssrs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `glasssrs` (
  `GlassTypeID` int NOT NULL,
  `GlassType` text,
  `Composition` text,
  `Type` text,
  `Total Thickness` double DEFAULT NULL,
  `U-value` double DEFAULT NULL,
  `Rw` int DEFAULT NULL,
  `Spacer` int DEFAULT NULL,
  `shgc` double DEFAULT NULL,
  `vt` double DEFAULT NULL,
  `rwc` int DEFAULT NULL,
  `rwctr` int DEFAULT NULL,
  `stc` int DEFAULT NULL,
  `oitc` int DEFAULT NULL,
  PRIMARY KEY (`GlassTypeID`),
  UNIQUE KEY `GlassTypeID_UNIQUE` (`GlassTypeID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `insulatingbar`
--

DROP TABLE IF EXISTS `insulatingbar`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `insulatingbar` (
  `InsulatingBarId` int NOT NULL AUTO_INCREMENT,
  `InsulatingBarGuid` char(36) DEFAULT NULL,
  `ArticleId` int NOT NULL,
  `ProductName` varchar(45) NOT NULL,
  `PTCoatedBefore` varchar(45) NOT NULL,
  `PTAnodizedBefore` varchar(45) NOT NULL,
  `PACoatedBefore` varchar(45) NOT NULL,
  `PACoatedAfter` varchar(45) NOT NULL,
  `PAAnodizedBefore` varchar(45) NOT NULL,
  `PAAnodizedAfter` varchar(45) NOT NULL,
  PRIMARY KEY (`InsulatingBarId`)
) ENGINE=InnoDB AUTO_INCREMENT=72 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
--
-- Table structure for table `order`
--

DROP TABLE IF EXISTS `order`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `order` (
  `OrderId` int NOT NULL AUTO_INCREMENT,
  `OrderExternalId` char(36) NOT NULL,
  `ProjectId` int NOT NULL,
  `DealerId` int NOT NULL,
  `ParentOrderId` int DEFAULT NULL,
  `CreatedBy` int NOT NULL,
  `CreatedOn` datetime NOT NULL,
  `ModifiedBy` int DEFAULT NULL,
  `ModifiedOn` datetime DEFAULT NULL,
  `Notes` text,
  `ShippingAddressId` int NOT NULL,
  `ShippingCost` double NOT NULL,
  `Tax` double NOT NULL,
  `Total` double NOT NULL,
  `Discount` double NOT NULL,
  `DiscountPercentage` int DEFAULT NULL,
  `ShippingMethod` tinyint DEFAULT NULL,
  PRIMARY KEY (`OrderId`),
  KEY `Order_ProjectId_idx` (`ProjectId`),
  KEY `Order_Dealer_idx` (`DealerId`),
  KEY `Order_ParentOrder_idx` (`ParentOrderId`),
  KEY `Order_ShippingAddress_idx` (`ShippingAddressId`),
  KEY `Order_CreatedBy_idx` (`CreatedBy`),
  KEY `Order_ModifiedBy_idx` (`ModifiedBy`),
  CONSTRAINT `Order_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `user` (`UserId`),
  CONSTRAINT `Order_Dealer` FOREIGN KEY (`DealerId`) REFERENCES `dealer` (`DealerId`),
  CONSTRAINT `Order_ModifiedBy` FOREIGN KEY (`ModifiedBy`) REFERENCES `user` (`UserId`),
  CONSTRAINT `Order_ParentOrder` FOREIGN KEY (`ParentOrderId`) REFERENCES `order` (`OrderId`),
  CONSTRAINT `Order_Project` FOREIGN KEY (`ProjectId`) REFERENCES `bpsproject` (`ProjectId`),
  CONSTRAINT `Order_ShippingAddress` FOREIGN KEY (`ShippingAddressId`) REFERENCES `address` (`AddressId`)
) ENGINE=InnoDB AUTO_INCREMENT=169 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `order_status`
--

DROP TABLE IF EXISTS `order_status`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `order_status` (
  `OrderId` int NOT NULL,
  `StatusId` int NOT NULL,
  `StatusModifiedOn` datetime NOT NULL,
  `StatusModifiedBy` int DEFAULT NULL,
  PRIMARY KEY (`OrderId`,`StatusId`),
  KEY `Order_Status_Status_idx` (`StatusId`),
  KEY `Order_Status_ModifiedBy_idx` (`StatusModifiedBy`),
  CONSTRAINT `Order_Status_ModifiedBy` FOREIGN KEY (`StatusModifiedBy`) REFERENCES `user` (`UserId`),
  CONSTRAINT `Order_Status_Order` FOREIGN KEY (`OrderId`) REFERENCES `order` (`OrderId`),
  CONSTRAINT `Order_Status_Status` FOREIGN KEY (`StatusId`) REFERENCES `slu_status` (`StatusId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `orderdetails`
--

DROP TABLE IF EXISTS `orderdetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orderdetails` (
  `OrderDetailId` int NOT NULL AUTO_INCREMENT,
  `OrderDetailExternalId` char(36) NOT NULL,
  `OrderId` int NOT NULL,
  `ProductId` int NOT NULL,
  `DesignURL` varchar(255) DEFAULT NULL,
  `JsonURL` varchar(255) DEFAULT NULL,
  `ProposalURL` varchar(255) DEFAULT NULL,
  `BomURL` varchar(255) DEFAULT NULL,
  `OrderDetailscol` varchar(45) DEFAULT NULL,
  `UnitPrice` varchar(45) DEFAULT NULL,
  `Qty` double DEFAULT NULL,
  `SubTotal` double DEFAULT NULL,
  `AdditionalDetails` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`OrderDetailId`),
  KEY `OrderDetails_OrderId_idx` (`OrderId`),
  KEY `OrderDetails_Product_idx` (`ProductId`),
  CONSTRAINT `OrderDetails_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `order` (`OrderId`),
  CONSTRAINT `OrderDetails_Product` FOREIGN KEY (`ProductId`) REFERENCES `bpsunifiedproblem` (`ProblemId`)
) ENGINE=InnoDB AUTO_INCREMENT=101 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `permission`
--

DROP TABLE IF EXISTS `permission`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permission` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PermissionGuid` char(36) NOT NULL,
  `Description` varchar(45) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `PermissionTypeGuid_UNIQUE` (`PermissionGuid`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `permission_role`
--

DROP TABLE IF EXISTS `permission_role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permission_role` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PermissionRoleGuid` char(36) NOT NULL,
  `UserRoleId` varchar(128) NOT NULL,
  `PermissionId` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `FeaturePermissionGuid_UNIQUE` (`PermissionRoleGuid`),
  KEY `UserRoleId_idx` (`UserRoleId`),
  KEY `PermissionTypeId_idx` (`PermissionId`),
  CONSTRAINT `PermissionId` FOREIGN KEY (`PermissionId`) REFERENCES `permission` (`Id`),
  CONSTRAINT `UserRoleId` FOREIGN KEY (`UserRoleId`) REFERENCES `aspnetroles` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=34 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `product`
--

DROP TABLE IF EXISTS `product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product` (
  `ProductId` int NOT NULL AUTO_INCREMENT,
  `ProductGuid` char(36) DEFAULT NULL,
  `Name` varchar(45) NOT NULL,
  `PrettyName` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`ProductId`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `productarticle`
--

DROP TABLE IF EXISTS `productarticle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `productarticle` (
  `ProductId` int NOT NULL,
  `ArticleId` int NOT NULL,
  PRIMARY KEY (`ProductId`,`ArticleId`),
  KEY `ProductArticle_ProductId_idx` (`ProductId`),
  KEY `ProductArticle_ArticleId` (`ArticleId`),
  CONSTRAINT `ProductArticle_ArticleId` FOREIGN KEY (`ArticleId`) REFERENCES `article` (`ArticleId`),
  CONSTRAINT `ProductArticle_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `product` (`ProductId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `producttype`
--

DROP TABLE IF EXISTS `producttype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `producttype` (
  `ProductTypeId` int NOT NULL AUTO_INCREMENT,
  `ProductCode` varchar(45) NOT NULL,
  `PrettyName` varchar(45) NOT NULL,
  PRIMARY KEY (`ProductTypeId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `slu_status`
--

DROP TABLE IF EXISTS `slu_status`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `slu_status` (
  `StatusId` int NOT NULL AUTO_INCREMENT,
  `StatusExternalId` char(36) NOT NULL,
  `Description` varchar(155) NOT NULL,
  `Code` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`StatusId`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `statetax`
--

DROP TABLE IF EXISTS `statetax`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `statetax` (
  `StateTaxId` int NOT NULL AUTO_INCREMENT,
  `StateTaxExternalId` char(36) NOT NULL,
  `Country` varchar(45) NOT NULL DEFAULT 'US',
  `State` varchar(45) NOT NULL,
  `ZipCode` varchar(45) NOT NULL,
  `TaxRegionName` varchar(255) NOT NULL,
  `StateRate` double DEFAULT '0',
  `EstimatedCombinedRate` double DEFAULT '0',
  `EstimatedCountyRate` double DEFAULT '0',
  `EstimatedCityRate` double DEFAULT '0',
  `EstimatedSpecialRate` double DEFAULT '0',
  `RiskLevel` double DEFAULT '0',
  PRIMARY KEY (`StateTaxId`)
) ENGINE=InnoDB AUTO_INCREMENT=39855 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `thermalbtobblockdata`
--

DROP TABLE IF EXISTS `thermalbtobblockdata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `thermalbtobblockdata` (
  `BlockID` int NOT NULL AUTO_INCREMENT,
  `Series` varchar(45) NOT NULL,
  `VentFWidth` double NOT NULL,
  `FixedorOperable` varchar(45) NOT NULL,
  `GlassThickness` double NOT NULL,
  `PAPT` varchar(45) NOT NULL,
  `k` double NOT NULL,
  `I` double NOT NULL,
  PRIMARY KEY (`BlockID`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `thermalbtobdirectdata`
--

DROP TABLE IF EXISTS `thermalbtobdirectdata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `thermalbtobdirectdata` (
  `DirectID` int NOT NULL AUTO_INCREMENT,
  `Series` varchar(45) NOT NULL,
  `GlazingInsulationType` varchar(45) NOT NULL,
  `FixedorOperable` varchar(45) NOT NULL,
  `GlassThickness` double NOT NULL,
  `PAPT` varchar(45) NOT NULL,
  `C382180` double NOT NULL,
  `C382200` double NOT NULL,
  `C382310` double NOT NULL,
  `C382330` double NOT NULL,
  `C382340` double NOT NULL,
  `C382110` double NOT NULL,
  `C382170` double NOT NULL,
  `C374980` double NOT NULL,
  PRIMARY KEY (`DirectID`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `thermalbtobstandarddata`
--

DROP TABLE IF EXISTS `thermalbtobstandarddata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `thermalbtobstandarddata` (
  `StandardID` int NOT NULL AUTO_INCREMENT,
  `Series` varchar(45) NOT NULL,
  `GlazingInsulationType` varchar(45) NOT NULL,
  `FixedorOperable` varchar(45) NOT NULL,
  `GlassThickness` double NOT NULL,
  `PAPT` varchar(45) NOT NULL,
  `k` double NOT NULL,
  `I` double NOT NULL,
  PRIMARY KEY (`StandardID`)
) ENGINE=InnoDB AUTO_INCREMENT=35 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `udcarticle`
--

DROP TABLE IF EXISTS `udcarticle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `udcarticle` (
  `ArticleSetID` int NOT NULL,
  `ArticleName` text,
  `Type` text,
  `AB` int DEFAULT NULL,
  `BT` int DEFAULT NULL,
  `BottomFraming` text,
  PRIMARY KEY (`ArticleSetID`),
  UNIQUE KEY `ArticleSetID_UNIQUE` (`ArticleSetID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `UserId` int NOT NULL AUTO_INCREMENT,
  `UserGuid` char(36) DEFAULT NULL,
  `UserName` varchar(45) NOT NULL,
  `NameFirst` varchar(45) DEFAULT NULL,
  `NameLast` varchar(45) DEFAULT NULL,
  `Email` varchar(45) DEFAULT NULL,
  `Language` varchar(45) DEFAULT NULL,
  `Company` varchar(45) DEFAULT NULL,
  `Hash` text,
  `Salt` blob,
  PRIMARY KEY (`UserId`),
  UNIQUE KEY `UserGuid_UNIQUE` (`UserGuid`)
) ENGINE=InnoDB AUTO_INCREMENT=128 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `useraccess`
--

DROP TABLE IF EXISTS `useraccess`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `useraccess` (
  `UserId` int NOT NULL,
  `AccessRoleId` int NOT NULL,
  PRIMARY KEY (`UserId`,`AccessRoleId`),
  KEY `UserId_idx` (`UserId`),
  KEY `AccessRoleId` (`AccessRoleId`),
  CONSTRAINT `AccessRoleId` FOREIGN KEY (`AccessRoleId`) REFERENCES `accessrole` (`AccessRoleId`),
  CONSTRAINT `UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `windzonegermany`
--

DROP TABLE IF EXISTS `windzonegermany`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `windzonegermany` (
  `PostCodeID` int NOT NULL,
  `PostCode` varchar(45) NOT NULL,
  `WindZone` int NOT NULL,
  `State` varchar(45) NOT NULL,
  `District` varchar(45) NOT NULL,
  `Place` varchar(45) NOT NULL,
  PRIMARY KEY (`PostCodeID`),
  UNIQUE KEY `PostCode_UNIQUE` (`PostCode`),
  UNIQUE KEY `PostCodeID_UNIQUE` (`PostCodeID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;