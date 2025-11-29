-- MySQL dump 10.13  Distrib 8.0.42, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: chat_app
-- ------------------------------------------------------
-- Server version	8.0.43

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
-- Table structure for table `usuarios_chats`
--

DROP TABLE IF EXISTS `usuarios_chats`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuarios_chats` (
  `id_usuario` int NOT NULL,
  `id_chat` int NOT NULL,
  PRIMARY KEY (`id_usuario`,`id_chat`),
  KEY `id_chat` (`id_chat`),
  CONSTRAINT `usuarios_chats_ibfk_1` FOREIGN KEY (`id_usuario`) REFERENCES `usuarios` (`id_usuario`),
  CONSTRAINT `usuarios_chats_ibfk_2` FOREIGN KEY (`id_chat`) REFERENCES `chats` (`id_chat`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuarios_chats`
--

LOCK TABLES `usuarios_chats` WRITE;
/*!40000 ALTER TABLE `usuarios_chats` DISABLE KEYS */;
INSERT INTO `usuarios_chats` VALUES (2,1),(4,1),(3,2),(4,2),(4,3),(5,3),(1,4),(4,4),(1,5),(6,5),(2,6),(7,6),(2,7),(3,7),(4,7),(5,7),(2,8),(4,8),(5,8),(4,12),(8,12),(3,13),(4,13),(5,13),(8,13),(4,14),(7,14),(1,15),(2,15),(4,15),(5,15),(5,16),(8,16),(3,17),(9,17),(1,18),(7,18),(1,19),(8,19),(1,20),(3,20),(1,21),(9,21),(1,22),(7,22),(8,22),(3,23),(8,23),(1,24),(3,24),(8,24),(1,25),(3,25),(8,25);
/*!40000 ALTER TABLE `usuarios_chats` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-11-29 17:32:33
