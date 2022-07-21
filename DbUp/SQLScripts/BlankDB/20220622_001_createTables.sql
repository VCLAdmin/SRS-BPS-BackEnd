CREATE DATABASE  IF NOT EXISTS `VCLDesignDb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `VCLDesignDB`;
-- MySQL dump 10.13  Distrib 8.0.18, for Win64 (x86_64)
--
-- Host: localhost    Database: VCLDesignDb
-- ------------------------------------------------------
-- Server version	8.0.23

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
-- Table structure for table `ADSArticle`
--

DROP TABLE IF EXISTS `ADSArticle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ADSArticle` (
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
-- Table structure for table `ASEArticle`
--

DROP TABLE IF EXISTS `ASEArticle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ASEArticle` (
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
-- Table structure for table `AccessRole`
--

DROP TABLE IF EXISTS `AccessRole`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AccessRole` (
  `AccessRoleId` int NOT NULL,
  `AccessRoleName` varchar(45) NOT NULL,
  PRIMARY KEY (`AccessRoleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `AcousticReport`
--

DROP TABLE IF EXISTS `AcousticReport`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AcousticReport` (
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



--
-- Table structure for table `Address`
--

DROP TABLE IF EXISTS `Address`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Address` (
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
  CONSTRAINT `Address_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `User` (`UserId`),
  CONSTRAINT `Address_ModifiedBy` FOREIGN KEY (`ModifiedBy`) REFERENCES `User` (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=1672 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `Article`
--

DROP TABLE IF EXISTS `Article`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Article` (
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
  CONSTRAINT `Article_ArticleTypeId` FOREIGN KEY (`ArticleTypeId`) REFERENCES `ArticleType` (`ArticleTypeId`)
) ENGINE=InnoDB AUTO_INCREMENT=238 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `ArticleSectionalProperty`
--

DROP TABLE IF EXISTS `ArticleSectionalProperty`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ArticleSectionalProperty` (
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
-- Table structure for table `ArticleType`
--

DROP TABLE IF EXISTS `ArticleType`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ArticleType` (
  `ArticleTypeId` int NOT NULL AUTO_INCREMENT,
  `ArticleTypeGuid` char(36) DEFAULT NULL,
  `PrettyName` varchar(45) NOT NULL,
  `Name` varchar(45) NOT NULL,
  PRIMARY KEY (`ArticleTypeId`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `AspNetRoles`
--

DROP TABLE IF EXISTS `AspNetRoles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AspNetRoles` (
  `Id` varchar(128) NOT NULL,
  `Name` varchar(256) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `AspNetUserClaims`
--

DROP TABLE IF EXISTS `AspNetUserClaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AspNetUserClaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` varchar(128) NOT NULL,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id` (`Id`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `ApplicationUser_Claims` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `AspNetUserLogins`
--

DROP TABLE IF EXISTS `AspNetUserLogins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AspNetUserLogins` (
  `LoginProvider` varchar(128) NOT NULL,
  `ProviderKey` varchar(128) NOT NULL,
  `UserId` varchar(128) NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`,`UserId`),
  KEY `ApplicationUser_Logins` (`UserId`),
  CONSTRAINT `ApplicationUser_Logins` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `AspNetUserRoles`
--

DROP TABLE IF EXISTS `AspNetUserRoles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AspNetUserRoles` (
  `UserId` varchar(128) NOT NULL,
  `RoleId` varchar(128) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IdentityRole_Users` (`RoleId`),
  CONSTRAINT `ApplicationUser_Roles` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `IdentityRole_Users` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `AspNetUsers`
--

DROP TABLE IF EXISTS `AspNetUsers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AspNetUsers` (
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
-- Table structure for table `BpsBoundaryCondition`
--

DROP TABLE IF EXISTS `BpsBoundaryCondition`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BpsBoundaryCondition` (
  `BoundaryConditionId` int NOT NULL AUTO_INCREMENT,
  `BoundaryConditionGuid` char(36) NOT NULL,
  `ProblemId` int NOT NULL,
  `BoundaryConditionNode` blob NOT NULL,
  PRIMARY KEY (`BoundaryConditionId`),
  UNIQUE KEY `BoundaryConditionGuid_UNIQUE` (`BoundaryConditionGuid`),
  KEY `FK_BpsBoundaryCondition` (`ProblemId`),
  CONSTRAINT `FK_BpsBoundaryCondition` FOREIGN KEY (`ProblemId`) REFERENCES `BpsProblem` (`ProblemId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `BpsProblem`
--

DROP TABLE IF EXISTS `BpsProblem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BpsProblem` (
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
  CONSTRAINT `BpsProblem_ProductTypeId` FOREIGN KEY (`ProductTypeId`) REFERENCES `ProductType` (`ProductTypeId`),
  CONSTRAINT `BpsProblem_ProjectId` FOREIGN KEY (`ProjectId`) REFERENCES `BpsProject` (`ProjectId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `BpsProject`
--

DROP TABLE IF EXISTS `BpsProject`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BpsProject` (
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
  CONSTRAINT `BpsProject_AddressId` FOREIGN KEY (`AddressId`) REFERENCES `Address` (`AddressId`),
  CONSTRAINT `BpsProject_UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=2551 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `BpsUnifiedProblem`
--

DROP TABLE IF EXISTS `BpsUnifiedProblem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BpsUnifiedProblem` (
  `ProblemId` int NOT NULL AUTO_INCREMENT,
  `ProblemGuid` char(36) NOT NULL,
  `ProblemName` varchar(45) NOT NULL,
  `ProjectId` int NOT NULL,
  `CreatedOn` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ModifiedOn` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `UnifiedModel` mediumtext NOT NULL,
  PRIMARY KEY (`ProblemId`),
  UNIQUE KEY `ProblemGuid_UNIQUE` (`ProblemGuid`)
) ENGINE=InnoDB AUTO_INCREMENT=5209 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `CustomArticle`
--

DROP TABLE IF EXISTS `CustomArticle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CustomArticle` (
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
  CONSTRAINT `CustomArticle_ArticleId` FOREIGN KEY (`ArticleId`) REFERENCES `Article` (`ArticleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `Dealer`
--

DROP TABLE IF EXISTS `Dealer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Dealer` (
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
  CONSTRAINT `Dealer_Address` FOREIGN KEY (`AddressId`) REFERENCES `Address` (`AddressId`),
  CONSTRAINT `Dealer_ADSFabricator` FOREIGN KEY (`ADSFabricatorId`) REFERENCES `Fabricator` (`FabricatorId`),
  CONSTRAINT `Dealer_ASSFabricator` FOREIGN KEY (`ASSFabricatorId`) REFERENCES `Fabricator` (`FabricatorId`),
  CONSTRAINT `Dealer_AWSFabricator` FOREIGN KEY (`AWSFabricatorId`) REFERENCES `Fabricator` (`FabricatorId`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `Dealer_User`
--

DROP TABLE IF EXISTS `Dealer_User`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Dealer_User` (
  `DealerId` int NOT NULL,
  `UserId` int NOT NULL,
  PRIMARY KEY (`DealerId`,`UserId`),
  KEY `UserId_FK_idx` (`UserId`),
  CONSTRAINT `DealerId_FK` FOREIGN KEY (`DealerId`) REFERENCES `Dealer` (`DealerId`),
  CONSTRAINT `UserId_FK` FOREIGN KEY (`UserId`) REFERENCES `User` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `DoorHandleHinge`
--

DROP TABLE IF EXISTS `DoorHandleHinge`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `DoorHandleHinge` (
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
-- Table structure for table `ExternalClient`
--

DROP TABLE IF EXISTS `ExternalClient`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ExternalClient` (
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
-- Table structure for table `Fabricator`
--

DROP TABLE IF EXISTS `Fabricator`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Fabricator` (
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
  CONSTRAINT `Fabricator_Address` FOREIGN KEY (`AddressId`) REFERENCES `Address` (`AddressId`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `Fabricator_User`
--

DROP TABLE IF EXISTS `Fabricator_User`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Fabricator_User` (
  `FabricatorId` int NOT NULL,
  `UserId` int NOT NULL,
  PRIMARY KEY (`FabricatorId`,`UserId`),
  KEY `Usr_UK_idx` (`UserId`),
  CONSTRAINT `Fab_FK` FOREIGN KEY (`FabricatorId`) REFERENCES `Fabricator` (`FabricatorId`),
  CONSTRAINT `Usr_UK` FOREIGN KEY (`UserId`) REFERENCES `User` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `FacadeArticle`
--

DROP TABLE IF EXISTS `FacadeArticle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `FacadeArticle` (
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
-- Table structure for table `FacadeArticleSectionalProperty`
--

DROP TABLE IF EXISTS `FacadeArticleSectionalProperty`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `FacadeArticleSectionalProperty` (
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
-- Table structure for table `FacadeInsertUnit`
--

DROP TABLE IF EXISTS `FacadeInsertUnit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `FacadeInsertUnit` (
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
-- Table structure for table `FacadeProfile`
--

DROP TABLE IF EXISTS `FacadeProfile`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `FacadeProfile` (
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
-- Table structure for table `FacadeSpacer`
--

DROP TABLE IF EXISTS `FacadeSpacer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `FacadeSpacer` (
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
-- Table structure for table `Feature`
--

DROP TABLE IF EXISTS `Feature`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Feature` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FeatureGuid` char(36) NOT NULL,
  `Feature` varchar(256) DEFAULT NULL,
  `ParentId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `FeatureGuid_UNIQUE` (`FeatureGuid`),
  KEY `ParentId_idx` (`ParentId`),
  CONSTRAINT `ParentId` FOREIGN KEY (`ParentId`) REFERENCES `Feature` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=118 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `Feature_Permission_Role`
--

DROP TABLE IF EXISTS `Feature_Permission_Role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Feature_Permission_Role` (
  `FeatureId` int NOT NULL,
  `PermissionRoleId` int NOT NULL,
  PRIMARY KEY (`FeatureId`,`PermissionRoleId`),
  KEY `PermissionRoleId_idx` (`PermissionRoleId`),
  CONSTRAINT `FeatureId` FOREIGN KEY (`FeatureId`) REFERENCES `Feature` (`Id`),
  CONSTRAINT `PermissionRoleId` FOREIGN KEY (`PermissionRoleId`) REFERENCES `Permission_Role` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `Financial`
--

DROP TABLE IF EXISTS `Financial`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Financial` (
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
  CONSTRAINT `Financial_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `User` (`UserId`),
  CONSTRAINT `Financial_Dealer` FOREIGN KEY (`DealerId`) REFERENCES `Dealer` (`DealerId`),
  CONSTRAINT `Financial_ModifiedBy` FOREIGN KEY (`ModifiedBy`) REFERENCES `User` (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `GlassBPS`
--

DROP TABLE IF EXISTS `GlassBPS`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GlassBPS` (
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
-- Table structure for table `GlassSRS`
--

DROP TABLE IF EXISTS `GlassSRS`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GlassSRS` (
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
-- Table structure for table `InsulatingBar`
--

DROP TABLE IF EXISTS `InsulatingBar`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `InsulatingBar` (
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
-- Table structure for table `Order`
--

DROP TABLE IF EXISTS `Order`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Order` (
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
  CONSTRAINT `Order_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `User` (`UserId`),
  CONSTRAINT `Order_Dealer` FOREIGN KEY (`DealerId`) REFERENCES `Dealer` (`DealerId`),
  CONSTRAINT `Order_ModifiedBy` FOREIGN KEY (`ModifiedBy`) REFERENCES `User` (`UserId`),
  CONSTRAINT `Order_ParentOrder` FOREIGN KEY (`ParentOrderId`) REFERENCES `Order` (`OrderId`),
  CONSTRAINT `Order_Project` FOREIGN KEY (`ProjectId`) REFERENCES `BpsProject` (`ProjectId`),
  CONSTRAINT `Order_ShippingAddress` FOREIGN KEY (`ShippingAddressId`) REFERENCES `Address` (`AddressId`)
) ENGINE=InnoDB AUTO_INCREMENT=1596 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `OrderDetails`
--

DROP TABLE IF EXISTS `OrderDetails`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `OrderDetails` (
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
  CONSTRAINT `OrderDetails_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `Order` (`OrderId`),
  CONSTRAINT `OrderDetails_Product` FOREIGN KEY (`ProductId`) REFERENCES `BpsUnifiedProblem` (`ProblemId`)
) ENGINE=InnoDB AUTO_INCREMENT=1184 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `Order_Status`
--

DROP TABLE IF EXISTS `Order_Status`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Order_Status` (
  `OrderId` int NOT NULL,
  `StatusId` int NOT NULL,
  `StatusModifiedOn` datetime NOT NULL,
  `StatusModifiedBy` int DEFAULT NULL,
  PRIMARY KEY (`OrderId`,`StatusId`),
  KEY `Order_Status_ModifiedBy_idx` (`StatusModifiedBy`),
  KEY `Order_Status_Status_idx` (`StatusId`),
  CONSTRAINT `Order_Status_ModifiedBy` FOREIGN KEY (`StatusModifiedBy`) REFERENCES `User` (`UserId`),
  CONSTRAINT `Order_Status_Order` FOREIGN KEY (`OrderId`) REFERENCES `Order` (`OrderId`),
  CONSTRAINT `Order_Status_Status_idx` FOREIGN KEY (`StatusId`) REFERENCES `SLU_Status` (`StatusId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `Permission`
--

DROP TABLE IF EXISTS `Permission`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Permission` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PermissionGuid` char(36) NOT NULL,
  `Description` varchar(45) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `PermissionTypeGuid_UNIQUE` (`PermissionGuid`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `Permission_Role`
--

DROP TABLE IF EXISTS `Permission_Role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Permission_Role` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PermissionRoleGuid` char(36) NOT NULL,
  `UserRoleId` varchar(128) NOT NULL,
  `PermissionId` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `FeaturePermissionGuid_UNIQUE` (`PermissionRoleGuid`),
  KEY `UserRoleId_idx` (`UserRoleId`),
  KEY `PermissionTypeId_idx` (`PermissionId`),
  CONSTRAINT `PermissionId` FOREIGN KEY (`PermissionId`) REFERENCES `Permission` (`Id`),
  CONSTRAINT `UserRoleId` FOREIGN KEY (`UserRoleId`) REFERENCES `AspNetRoles` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=34 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `Product`
--

DROP TABLE IF EXISTS `Product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Product` (
  `ProductId` int NOT NULL AUTO_INCREMENT,
  `ProductGuid` char(36) DEFAULT NULL,
  `Name` varchar(45) NOT NULL,
  `PrettyName` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`ProductId`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `ProductArticle`
--

DROP TABLE IF EXISTS `ProductArticle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ProductArticle` (
  `ProductId` int NOT NULL,
  `ArticleId` int NOT NULL,
  PRIMARY KEY (`ProductId`,`ArticleId`),
  KEY `ProductArticle_ProductId_idx` (`ProductId`),
  KEY `ProductArticle_ArticleId` (`ArticleId`),
  CONSTRAINT `ProductArticle_ArticleId` FOREIGN KEY (`ArticleId`) REFERENCES `Article` (`ArticleId`),
  CONSTRAINT `ProductArticle_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Product` (`ProductId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `ProductType`
--

DROP TABLE IF EXISTS `ProductType`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ProductType` (
  `ProductTypeId` int NOT NULL AUTO_INCREMENT,
  `ProductCode` varchar(45) NOT NULL,
  `PrettyName` varchar(45) NOT NULL,
  PRIMARY KEY (`ProductTypeId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `SLU_Status`
--

DROP TABLE IF EXISTS `SLU_Status`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `SLU_Status` (
  `StatusId` int NOT NULL AUTO_INCREMENT,
  `StatusExternalId` char(36) NOT NULL,
  `Description` varchar(155) NOT NULL,
  `Code` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`StatusId`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `StateTax`
--

DROP TABLE IF EXISTS `StateTax`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `StateTax` (
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
-- Table structure for table `ThermalBtoBBlockData`
--

DROP TABLE IF EXISTS `ThermalBtoBBlockData`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ThermalBtoBBlockData` (
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
-- Table structure for table `ThermalBtoBDirectData`
--

DROP TABLE IF EXISTS `ThermalBtoBDirectData`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ThermalBtoBDirectData` (
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
-- Table structure for table `ThermalBtoBStandardData`
--

DROP TABLE IF EXISTS `ThermalBtoBStandardData`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ThermalBtoBStandardData` (
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
-- Table structure for table `UDCArticle`
--

DROP TABLE IF EXISTS `UDCArticle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `UDCArticle` (
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
-- Table structure for table `User`
--

DROP TABLE IF EXISTS `User`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `User` (
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
) ENGINE=InnoDB AUTO_INCREMENT=147 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `UserAccess`
--

DROP TABLE IF EXISTS `UserAccess`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `UserAccess` (
  `UserId` int NOT NULL,
  `AccessRoleId` int NOT NULL,
  PRIMARY KEY (`UserId`,`AccessRoleId`),
  KEY `UserId_idx` (`UserId`),
  KEY `AccessRoleId` (`AccessRoleId`),
  CONSTRAINT `AccessRoleId` FOREIGN KEY (`AccessRoleId`) REFERENCES `AccessRole` (`AccessRoleId`),
  CONSTRAINT `UserId` FOREIGN KEY (`UserId`) REFERENCES `User` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;



--
-- Table structure for table `WindZoneGermany`
--

DROP TABLE IF EXISTS `WindZoneGermany`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WindZoneGermany` (
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


SET @@SESSION.SQL_LOG_BIN = @MYSQLDUMP_TEMP_LOG_BIN;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2022-06-22 17:28:28
