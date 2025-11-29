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
-- Table structure for table `mensajes`
--

DROP TABLE IF EXISTS `mensajes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `mensajes` (
  `id_mensaje` int NOT NULL AUTO_INCREMENT,
  `id_chat` int DEFAULT NULL,
  `id_usuario` int DEFAULT NULL,
  `mensaje` text NOT NULL,
  `fecha` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_mensaje`),
  KEY `id_chat` (`id_chat`),
  KEY `id_usuario` (`id_usuario`),
  CONSTRAINT `mensajes_ibfk_1` FOREIGN KEY (`id_chat`) REFERENCES `chats` (`id_chat`),
  CONSTRAINT `mensajes_ibfk_2` FOREIGN KEY (`id_usuario`) REFERENCES `usuarios` (`id_usuario`)
) ENGINE=InnoDB AUTO_INCREMENT=68 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `mensajes`
--

LOCK TABLES `mensajes` WRITE;
/*!40000 ALTER TABLE `mensajes` DISABLE KEYS */;
INSERT INTO `mensajes` VALUES (1,4,4,'hola','2025-10-13 17:22:39'),(2,4,1,'hola, como estas?','2025-10-13 17:23:50'),(3,3,4,'{\\rtf1\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang3082{\\fonttbl{\\f0\\fnil\\fcharset0 Microsoft Sans Serif;}}\r\n{\\*\\generator Riched20 10.0.26100}\\viewkind4\\uc1 \r\n\\pard\\f0\\fs17\\lang1033 holaa\\par\r\n\\lang3082\\par\r\n}\r\n','2025-10-13 22:52:35'),(4,7,4,'{\\rtf1\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang3082{\\fonttbl{\\f0\\fnil\\fcharset0 Microsoft Sans Serif;}}\r\n{\\*\\generator Riched20 10.0.26100}\\viewkind4\\uc1 \r\n\\pard\\f0\\fs17\\lang1033 hola amigos\\par\r\n\\lang3082\\par\r\n}\r\n','2025-10-13 22:52:58'),(5,7,4,'{\\rtf1\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang3082{\\fonttbl{\\f0\\fnil\\fcharset0 Microsoft Sans Serif;}}\r\n{\\*\\generator Riched20 10.0.26100}\\viewkind4\\uc1 \r\n\\pard\\f0\\fs17\\lang1033 como estan?\\par\r\n\\lang3082\\par\r\n}\r\n','2025-10-13 22:53:09'),(6,4,4,'{\\rtf1\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang3082{\\fonttbl{\\f0\\fnil\\fcharset0 Microsoft Sans Serif;}}\r\n{\\*\\generator Riched20 10.0.26100}\\viewkind4\\uc1 \r\n\\pard\\f0\\fs17\\lang1033 hola\\par\r\n\\lang3082\\par\r\n}\r\n','2025-10-13 22:53:19'),(7,7,4,'hola','2025-10-13 22:58:39'),(8,4,4,'bien y tu?','2025-10-13 23:02:32'),(9,8,4,'hola','2025-10-13 23:14:07'),(10,8,4,'holaa','2025-10-14 00:14:14'),(11,8,4,'hola','2025-10-14 09:30:11'),(12,1,4,'Hoa como estas?','2025-10-14 09:39:07'),(13,1,4,'<3','2025-10-14 09:39:08'),(14,1,4,':(','2025-10-14 09:39:31'),(15,12,4,'hola','2025-10-14 10:13:36'),(16,13,4,'hola','2025-10-14 10:14:12'),(17,8,4,'que onda','2025-10-14 10:31:55'),(18,15,4,'holaaaa','2025-10-14 10:33:03'),(19,15,4,'que onda','2025-10-14 10:34:52'),(20,15,1,'que onda como estas?','2025-10-14 10:35:52'),(21,12,4,'que onda joto','2025-10-14 10:36:55'),(22,19,1,'Hola a todooooooooooooooooooos','2025-11-26 17:20:10'),(23,15,4,'hola','2025-11-26 18:11:27'),(24,19,1,'Hola a todos','2025-11-26 18:17:44'),(25,19,1,'Hola','2025-11-26 18:23:06'),(26,5,1,'hola','2025-11-26 18:37:38'),(27,15,1,'hola','2025-11-26 18:41:47'),(28,20,1,'hola','2025-11-26 18:46:42'),(29,20,1,'fdf','2025-11-26 18:46:46'),(30,19,1,'holiwiss','2025-11-26 19:03:36'),(31,20,1,'Holiwis','2025-11-26 19:07:02'),(32,19,1,':)','2025-11-26 19:48:56'),(33,20,1,'hola','2025-11-26 21:07:05'),(34,20,1,'?','2025-11-26 21:07:13'),(35,21,1,'hola cobija','2025-11-26 21:08:51'),(36,21,1,'como estas ?','2025-11-26 21:08:58'),(37,21,1,'❤️','2025-11-27 10:30:16'),(38,4,1,'HOLA ?','2025-11-27 10:31:05'),(39,4,4,'Hola si','2025-11-27 10:31:23'),(40,15,1,'holaaaaa','2025-11-27 10:32:14'),(41,15,4,'si, como estas?','2025-11-27 10:32:30'),(42,15,1,'HOLA','2025-11-27 10:53:11'),(43,4,4,'QUe onda como estas','2025-11-27 10:53:24'),(44,20,1,'HOLAAA ?','2025-11-27 10:56:20'),(45,4,4,'Hoy es Viernes  ?','2025-11-28 10:36:46'),(46,4,1,'hola','2025-11-28 10:39:08'),(47,4,4,'? ? ?','2025-11-28 10:39:35'),(48,19,8,'hola hoy','2025-11-28 10:40:37'),(49,12,4,'xd','2025-11-28 10:40:47'),(50,13,4,'Holaaa ?','2025-11-28 10:42:15'),(51,13,4,'Holaaa ?','2025-11-28 10:42:17'),(52,15,4,'Holaaa ?','2025-11-28 10:44:13'),(53,24,3,'hola','2025-11-28 10:48:42'),(54,24,8,'Holaaa ?','2025-11-28 10:48:44'),(55,24,1,'holaaa','2025-11-28 10:49:22'),(56,24,1,'holaaa','2025-11-28 10:49:22'),(57,24,8,'Hola u1','2025-11-28 10:49:36'),(58,24,1,'aaaa','2025-11-28 10:49:50'),(59,24,8,'Como estan? ?','2025-11-28 10:51:20'),(60,24,8,'hola otra vez','2025-11-28 10:59:53'),(61,24,1,'asasaasa','2025-11-28 10:59:59'),(62,24,1,'select * from usuarios','2025-11-28 11:00:12'),(63,24,8,'Hoy es viernes 28 de noviembre asi es ? ?','2025-11-28 11:01:17'),(64,24,3,'Hola a las 110','2025-11-28 11:06:15'),(65,24,8,'hola amigo','2025-11-28 11:06:22'),(66,24,1,'hoaa','2025-11-28 11:06:38'),(67,25,8,'hola','2025-11-28 11:07:17');
/*!40000 ALTER TABLE `mensajes` ENABLE KEYS */;
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
