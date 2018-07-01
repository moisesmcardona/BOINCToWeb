CREATE DATABASE  IF NOT EXISTS `boincweb` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `boincweb`;
-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Database: boincweb
-- ------------------------------------------------------
-- Server version	5.7.18-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `tasks`
--

DROP TABLE IF EXISTS `tasks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tasks` (
  `TaskName` varchar(256) DEFAULT NULL,
  `Project` varchar(256) DEFAULT NULL,
  `PercentDone` varchar(45) DEFAULT NULL,
  `Status` varchar(45) DEFAULT NULL,
  `Pkey` int(11) NOT NULL AUTO_INCREMENT,
  `PCName` varchar(100) DEFAULT NULL,
  `ElapsedTime` varchar(45) DEFAULT NULL,
  `RemainingTime` varchar(45) DEFAULT NULL,
  `ReportDeadline` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Pkey`)
) ENGINE=InnoDB AUTO_INCREMENT=691 DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `finishedtasks`;
CREATE TABLE `finishedtasks` (
  `PKey` int(11) NOT NULL AUTO_INCREMENT,
  `Project` varchar(256) DEFAULT NULL,
  `TaskName` varchar(256) DEFAULT NULL,
  `PCName` varchar(256) DEFAULT NULL,
  `ElapsedTime` varchar(45) DEFAULT NULL,
  `PlanClass` varchar(45) DEFAULT NULL,
  `AddedDate` varchar(45) DEFAULT NULL,
  `AddedTime` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`PKey`),
  KEY `ProjectIndex` (`Project`),
  KEY `PCNameIndex` (`PCName`),
  KEY `PlanClassIndex` (`PlanClass`),
  KEY `DateIndex` (`AddedDate`)
) ENGINE=InnoDB AUTO_INCREMENT=24 DEFAULT CHARSET=utf8;

/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-04-23 18:48:56
