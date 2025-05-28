-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3307
-- Generation Time: May 21, 2025 at 07:24 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `healthymealplanner`
--

-- --------------------------------------------------------

--
-- Table structure for table `allergies`
--

CREATE TABLE `allergies` (
  `AllergyID` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

--
-- Dumping data for table `allergies`
--

INSERT INTO `allergies` (`AllergyID`, `Name`) VALUES
(1, 'Egg'),
(2, 'Milk'),
(3, 'Nuts'),
(4, 'Soybean'),
(5, 'Fish'),
(6, 'Wheat'),
(7, 'Celery'),
(8, 'Shellfish'),
(9, 'Sesame');

-- --------------------------------------------------------

--
-- Table structure for table `categories`
--

CREATE TABLE `categories` (
  `CategoryID` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Description` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `categories`
--

INSERT INTO `categories` (`CategoryID`, `Name`, `Description`) VALUES
(1, 'Omnivore', 'No specific dietary preferences'),
(2, 'Vegetarian', 'Vegetarian recipes'),
(3, 'Vegan', 'Vegan recipes'),
(4, 'Keto', 'Low-carbohydrate recipes'),
(5, 'Paleo', 'Unprocessed foods');

-- --------------------------------------------------------

--
-- Table structure for table `emailsettings`
--

CREATE TABLE `emailsettings` (
  `Id` int(11) NOT NULL,
  `FromEmail` varchar(255) NOT NULL,
  `AppPassword` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

--
-- Dumping data for table `emailsettings`
--

INSERT INTO `emailsettings` (`Id`, `FromEmail`, `AppPassword`) VALUES
(1, 'j00537208@gmail.com', 'uorm hwbf cuma mdra');

-- --------------------------------------------------------

--
-- Table structure for table `favorites`
--

CREATE TABLE `favorites` (
  `UserID` int(11) NOT NULL,
  `RecipeID` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- --------------------------------------------------------

--
-- Table structure for table `ingredients`
--

CREATE TABLE `ingredients` (
  `IngredientID` int(11) NOT NULL,
  `Name` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `ingredients`
--

INSERT INTO `ingredients` (`IngredientID`, `Name`) VALUES
(27, 'alfredo sauce'),
(174, 'all-purpose flour'),
(12, 'almonds'),
(125, 'andouille sausage'),
(42, 'asparagus'),
(7, 'avocado'),
(30, 'bacon'),
(207, 'baking powder'),
(54, 'balsamic vinegar'),
(108, 'barbecue sauce'),
(45, 'basil'),
(275, 'bay leaf'),
(142, 'beef bouillon cube'),
(146, 'beef broth'),
(208, 'beer'),
(252, 'bell peppers'),
(11, 'berries'),
(37, 'black pepper'),
(182, 'bread'),
(75, 'bread crumbs'),
(49, 'breadcrumbs'),
(2, 'broccoli'),
(161, 'broccoli florets'),
(3, 'brown rice'),
(267, 'brown sugar'),
(189, 'butter'),
(216, 'cabbage'),
(201, 'caesar dressing'),
(124, 'cajun seasoning'),
(160, 'canola oil'),
(212, 'capers'),
(53, 'carrot'),
(90, 'cayenne pepper'),
(52, 'celery'),
(148, 'cheddar cheese'),
(9, 'chicken breast'),
(130, 'chicken broth'),
(264, 'chicken stock'),
(251, 'chicken tenders'),
(247, 'chili powder'),
(150, 'chives'),
(278, 'chorizo sausage'),
(96, 'cilantro'),
(214, 'cod fillets'),
(215, 'corn tortillas'),
(157, 'cornstarch'),
(106, 'country-style pork ribs'),
(33, 'cream of mushroom soup'),
(69, 'creole seasoning'),
(55, 'crushed tomatoes'),
(91, 'cumin'),
(266, 'curry powder'),
(239, 'dark brown sugar'),
(185, 'deli turkey breast'),
(213, 'dill weed'),
(248, 'dried oregano'),
(73, 'dry bread stuffing mix'),
(144, 'dry mustard'),
(209, 'egg'),
(31, 'egg noodles'),
(111, 'eggs'),
(70, 'file powder'),
(93, 'flank steak'),
(14, 'flatbread'),
(112, 'flour'),
(231, 'flour tortillas'),
(158, 'fresh ginger'),
(29, 'fresh mushrooms'),
(26, 'frozen bread dough'),
(43, 'garlic'),
(17, 'garlic cloves'),
(34, 'garlic powder'),
(238, 'ginger'),
(263, 'granny smith apple'),
(10, 'greek yogurt'),
(67, 'green bell pepper'),
(74, 'green bell peppers'),
(230, 'green chile peppers'),
(241, 'green onions'),
(32, 'ground beef'),
(140, 'ground chuck'),
(250, 'ground cumin'),
(71, 'ham'),
(291, 'heavy cream'),
(13, 'honey'),
(293, 'horseradish'),
(127, 'hot sauce'),
(25, 'italian dressing'),
(175, 'italian-style seasoned bread crumbs'),
(95, 'jalapeno'),
(162, 'jasmine rice'),
(145, 'ketchup'),
(107, 'kosher salt'),
(261, 'lamb stew meat'),
(46, 'lemon juice'),
(48, 'lemon zest'),
(184, 'lettuce'),
(143, 'light brown sugar'),
(211, 'lime'),
(87, 'lime juice'),
(188, 'linguine pasta'),
(94, 'mango'),
(292, 'maple syrup'),
(183, 'mayonnaise'),
(176, 'milk'),
(149, 'monterey jack cheese'),
(114, 'mozzarella'),
(28, 'mozzarella cheese'),
(294, 'mustard powder'),
(217, 'oil for frying'),
(4, 'olive oil'),
(51, 'onion'),
(249, 'onion powder'),
(18, 'oregano'),
(92, 'paprika'),
(44, 'parmesan'),
(202, 'parmesan cheese'),
(68, 'parsley'),
(123, 'peanut oil'),
(16, 'pepperoncini'),
(210, 'plain yogurt'),
(172, 'pork chops'),
(115, 'provolone'),
(5, 'quinoa'),
(265, 'raisins'),
(277, 'red bell pepper'),
(15, 'red onion'),
(126, 'red pepper flakes'),
(147, 'refrigerated mashed potatoes'),
(159, 'rice vinegar'),
(228, 'roast beef'),
(200, 'romaine lettuce'),
(274, 'saffron threads'),
(39, 'salmon'),
(36, 'salt'),
(163, 'scallions'),
(173, 'seasoning salt'),
(84, 'sesame oil'),
(190, 'shallots'),
(273, 'short-grain white rice'),
(72, 'shrimp'),
(35, 'sour cream'),
(86, 'soy sauce'),
(50, 'spaghetti'),
(276, 'spanish onion'),
(8, 'spinach'),
(57, 'sugar'),
(6, 'sweet potatoes'),
(229, 'taco sauce'),
(83, 'teriyaki sauce'),
(47, 'thyme'),
(164, 'toasted sesame seeds'),
(1, 'tofu'),
(186, 'tomato'),
(56, 'tomato paste'),
(113, 'tomato sauce'),
(227, 'tomatoes'),
(97, 'tortillas'),
(262, 'unsalted butter'),
(226, 'vegetable oil'),
(88, 'vinegar'),
(295, 'waffles'),
(240, 'water'),
(89, 'white pepper'),
(129, 'white rice'),
(177, 'white wine'),
(128, 'worcestershire sauce'),
(141, 'yellow onion');

-- --------------------------------------------------------

--
-- Table structure for table `mealplanitems`
--

CREATE TABLE `mealplanitems` (
  `MealPlanItemID` int(11) NOT NULL,
  `MealPlanID` int(11) DEFAULT NULL,
  `RecipeID` int(11) DEFAULT NULL,
  `MealType` enum('Breakfast','Lunch','Dinner','Snack') DEFAULT NULL,
  `DayOfWeek` enum('Monday','Tuesday','Wednesday','Thursday','Friday','Saturday','Sunday') DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `mealplans`
--

CREATE TABLE `mealplans` (
  `MealPlanID` int(11) NOT NULL,
  `UserID` int(11) DEFAULT NULL,
  `Name` varchar(100) NOT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `recipeallergies`
--

CREATE TABLE `recipeallergies` (
  `RecipeID` int(11) NOT NULL,
  `AllergyID` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

--
-- Dumping data for table `recipeallergies`
--

INSERT INTO `recipeallergies` (`RecipeID`, `AllergyID`) VALUES
(1, 4),
(2, 3),
(4, 2),
(4, 3),
(5, 2),
(5, 6),
(7, 2),
(7, 6),
(8, 2),
(8, 6),
(9, 5),
(10, 6),
(10, 7),
(12, 4),
(12, 9),
(13, 4),
(13, 6),
(15, 1),
(15, 2),
(15, 6),
(16, 7),
(16, 8),
(17, 2),
(17, 6),
(18, 4),
(19, 2),
(19, 6),
(20, 1),
(20, 2),
(20, 6),
(21, 6),
(21, 8),
(22, 1),
(22, 2),
(23, 1),
(23, 5),
(23, 6),
(24, 6),
(25, 4),
(26, 6),
(26, 7),
(27, 2),
(27, 7),
(28, 5),
(28, 7),
(28, 8),
(29, 1),
(29, 2),
(29, 4),
(29, 6);

-- --------------------------------------------------------

--
-- Table structure for table `recipecategories`
--

CREATE TABLE `recipecategories` (
  `RecipeID` int(11) NOT NULL,
  `CategoryID` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `recipeingredients`
--

CREATE TABLE `recipeingredients` (
  `RecipeID` int(11) NOT NULL,
  `IngredientID` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `recipeingredients`
--

INSERT INTO `recipeingredients` (`RecipeID`, `IngredientID`) VALUES
(1, 1),
(1, 2),
(1, 3),
(1, 4),
(2, 4),
(2, 5),
(2, 6),
(2, 7),
(2, 8),
(3, 4),
(3, 7),
(3, 8),
(3, 9),
(4, 10),
(4, 11),
(4, 12),
(4, 13),
(5, 14),
(5, 15),
(5, 16),
(5, 17),
(5, 18),
(7, 9),
(7, 25),
(7, 26),
(7, 27),
(7, 28),
(7, 29),
(7, 30),
(8, 31),
(8, 32),
(8, 33),
(8, 34),
(8, 35),
(8, 36),
(8, 37),
(9, 4),
(9, 36),
(9, 37),
(9, 39),
(9, 42),
(9, 43),
(9, 44),
(9, 45),
(9, 46),
(9, 47),
(9, 48),
(9, 49),
(10, 4),
(10, 18),
(10, 30),
(10, 32),
(10, 36),
(10, 37),
(10, 43),
(10, 44),
(10, 45),
(10, 50),
(10, 51),
(10, 52),
(10, 53),
(10, 54),
(10, 55),
(10, 56),
(10, 57),
(11, 4),
(11, 32),
(11, 36),
(11, 37),
(11, 43),
(11, 49),
(11, 51),
(11, 52),
(11, 67),
(11, 68),
(11, 69),
(11, 70),
(11, 72),
(12, 9),
(12, 43),
(12, 46),
(12, 83),
(12, 84),
(13, 4),
(13, 7),
(13, 15),
(13, 18),
(13, 34),
(13, 36),
(13, 37),
(13, 43),
(13, 86),
(13, 87),
(13, 88),
(13, 89),
(13, 90),
(13, 91),
(13, 92),
(13, 93),
(13, 94),
(13, 95),
(13, 96),
(13, 97),
(14, 34),
(14, 37),
(14, 106),
(14, 107),
(14, 108),
(15, 4),
(15, 9),
(15, 36),
(15, 37),
(15, 44),
(15, 45),
(15, 49),
(15, 111),
(15, 112),
(15, 113),
(15, 114),
(15, 115),
(16, 9),
(16, 36),
(16, 37),
(16, 43),
(16, 51),
(16, 52),
(16, 55),
(16, 67),
(16, 70),
(16, 123),
(16, 124),
(16, 125),
(16, 126),
(16, 127),
(16, 128),
(16, 129),
(16, 130),
(17, 4),
(17, 37),
(17, 43),
(17, 55),
(17, 56),
(17, 128),
(17, 140),
(17, 141),
(17, 142),
(17, 143),
(17, 144),
(17, 145),
(17, 146),
(17, 147),
(17, 148),
(17, 149),
(17, 150),
(18, 32),
(18, 43),
(18, 86),
(18, 107),
(18, 141),
(18, 143),
(18, 146),
(18, 157),
(18, 158),
(18, 159),
(18, 160),
(18, 161),
(18, 162),
(18, 163),
(18, 164),
(19, 4),
(19, 33),
(19, 34),
(19, 111),
(19, 172),
(19, 173),
(19, 174),
(19, 175),
(19, 176),
(19, 177),
(20, 30),
(20, 182),
(20, 183),
(20, 184),
(20, 185),
(20, 186),
(21, 4),
(21, 37),
(21, 43),
(21, 46),
(21, 68),
(21, 72),
(21, 107),
(21, 126),
(21, 177),
(21, 188),
(21, 189),
(21, 190),
(22, 4),
(22, 9),
(22, 30),
(22, 124),
(22, 200),
(22, 201),
(22, 202),
(23, 18),
(23, 36),
(23, 90),
(23, 91),
(23, 95),
(23, 157),
(23, 174),
(23, 183),
(23, 207),
(23, 208),
(23, 209),
(23, 210),
(23, 211),
(23, 212),
(23, 213),
(23, 214),
(23, 215),
(23, 216),
(23, 217),
(24, 43),
(24, 51),
(24, 91),
(24, 126),
(24, 148),
(24, 184),
(24, 226),
(24, 227),
(24, 228),
(24, 229),
(24, 230),
(24, 231),
(25, 43),
(25, 86),
(25, 93),
(25, 157),
(25, 226),
(25, 238),
(25, 239),
(25, 240),
(25, 241),
(26, 34),
(26, 36),
(26, 37),
(26, 51),
(26, 90),
(26, 96),
(26, 211),
(26, 226),
(26, 247),
(26, 248),
(26, 249),
(26, 250),
(26, 251),
(26, 252),
(27, 36),
(27, 37),
(27, 46),
(27, 51),
(27, 174),
(27, 261),
(27, 262),
(27, 263),
(27, 264),
(27, 265),
(27, 266),
(27, 267),
(28, 4),
(28, 9),
(28, 18),
(28, 36),
(28, 37),
(28, 43),
(28, 48),
(28, 68),
(28, 72),
(28, 92),
(28, 126),
(28, 264),
(28, 273),
(28, 274),
(28, 275),
(28, 276),
(28, 277),
(28, 278),
(29, 30),
(29, 36),
(29, 37),
(29, 90),
(29, 111),
(29, 123),
(29, 148),
(29, 157),
(29, 174),
(29, 183),
(29, 251),
(29, 291),
(29, 292),
(29, 293),
(29, 294),
(29, 295);

-- --------------------------------------------------------

--
-- Table structure for table `recipes`
--

CREATE TABLE `recipes` (
  `RecipeID` int(11) NOT NULL,
  `Title` varchar(100) NOT NULL,
  `Description` text DEFAULT NULL,
  `Instructions` text DEFAULT NULL,
  `PrepTime` int(11) DEFAULT NULL,
  `CookTime` int(11) DEFAULT NULL,
  `Servings` int(11) DEFAULT NULL,
  `CreatedAt` timestamp NOT NULL DEFAULT current_timestamp(),
  `UpdatedAt` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `ImagePath` varchar(255) DEFAULT NULL,
  `Calories` int(11) DEFAULT NULL,
  `CategoryID` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `recipes`
--

INSERT INTO `recipes` (`RecipeID`, `Title`, `Description`, `Instructions`, `PrepTime`, `CookTime`, `Servings`, `CreatedAt`, `UpdatedAt`, `ImagePath`, `Calories`, `CategoryID`) VALUES
(1, 'Tofu Stir-Fry', 'Protein-rich tofu stir-fry with brown rice and vegetables', 'Press tofu for 30 minutes and cube\nCook brown rice according to package instructions\nCut broccoli into florets\nStir-fry tofu until golden\nAdd broccoli and cook until tender-crisp', 35, 20, 1, '2025-05-19 12:47:33', '2025-05-19 23:14:12', '/Recipes/tofu_stirfry.jpg', 320, 3),
(2, 'Quinoa Power Bowl', 'Nutritious quinoa bowl with roasted vegetables and avocado', 'Rinse 1 cup quinoa and cook with 2 cups water for 15-20 minutes\nCube sweet potatoes and roast with olive oil at 400°F for 25 minutes\nWash and prepare spinach\nSlice avocado\nCombine all ingredients in a bowl and drizzle with olive oil', 10, 25, 1, '2025-05-19 12:47:33', '2025-05-19 23:14:18', '/Recipes/quinoa_bowl.jpg', 450, 2),
(3, 'Grilled Chicken Salad', 'Fresh salad with grilled chicken breast and avocado', 'Season chicken breast with salt and pepper\nGrill chicken for 6-8 minutes per side\nWash and prepare spinach\nSlice avocado\nCombine ingredients and drizzle with olive oil', 10, 16, 1, '2025-05-19 12:47:33', '2025-05-19 23:14:22', '/Recipes/grilled_chicken_salad.jpg', 380, 1),
(4, 'Greek Yogurt Breakfast Bowl', 'Protein-packed breakfast with berries and almonds', 'Add Greek yogurt to a bowl\nTop with fresh berries\nSprinkle with chopped almonds\nDrizzle with honey if desired', 5, 0, 1, '2025-05-19 13:17:09', '2025-05-19 23:14:37', '/Recipes/greek_yogurt_bowl.jpg', 280, 2),
(5, 'Mediterranean Flatbread', 'They look a little fancy, but are uncomplicated and perfect for a quick meal or appetizer.', 'Preheat the oven to 425 degrees F (220 degrees C).\r\n\r\nCombine olive oil and garlic in a small bowl. Lightly brush a little garlic olive oil over the flatbreads.\r\n\r\nAdd tomatoes, olives, onion, and pepperoncini to a bowl; drizzle with remaining garlic olive oil, and gently combine.\r\n\r\nEvenly top flatbreads with the tomato mixture, sprinkle with feta cheese, and place on a baking sheet.\r\n\r\nBake. in the preheated oven until flatbreads are starting to turn golden brown, 12 to 15 minutes. Sprinkle with chopped oregano and serve.', 15, 15, 4, '2025-05-19 16:15:21', '2025-05-19 23:14:28', '/Recipes/flatbread.jpg', 373, 1),
(7, 'Grilled Chicken Alfredo Flatbread Pizzas', 'Delicious flatbread pizzas topped with Alfredo sauce, grilled chicken, mushrooms, and mozzarella cheese.', 'Marinate chicken in Italian dressing for 10 minutes and slice thinly.\nDivide thawed dough into 2 pieces and roll into 9-inch circles.\nSpread Alfredo sauce over each, top with mozzarella, chicken, mushrooms, and bacon.\nSprinkle remaining cheese on top.\nBake in a preheated oven or a Panasonic CIO on Frozen Pizza setting for ~13 minutes each.', 15, 30, 4, '2025-05-19 23:42:43', '2025-05-20 00:34:45', '/Recipes/flatbread_pizza.jpg', 580, 1),
(8, 'Simple Beef Stroganoff', 'A quick and creamy ground beef stroganoff made with pantry staples. Ready in just 20 minutes.', 'Sauté ground beef in a skillet over medium heat until browned (5–10 mins), then drain excess fat.\nMeanwhile, boil egg noodles until al dente (7–9 mins), then drain.\nStir mushroom soup and garlic powder into the beef; simmer for 10 mins, stirring occasionally.\nRemove from heat. Add cooked noodles and stir in sour cream.\nSeason with salt and pepper. Serve hot and garnish with parsley if desired.', 5, 15, 4, '2025-05-19 21:47:26', '2025-05-20 00:34:53', '/Recipes/beef_stroganoff.jpg', 679, 1),
(9, 'Baked Salmon with Basil and Lemon Thyme Crust', 'Perfectly baked salmon with a crispy, lemony herb crust, served with asparagus.', 'Rub some olive oil on the salmon and season with salt and pepper. Place it on a pan.\nMix the asparagus with olive oil, salt, and pepper and place it around the salmon.\nBlend garlic, Parmesan, basil, lemon juice, thyme, lemon zest, salt, pepper, olive oil, and breadcrumbs.\nSpread the mixture over the salmon as a crust.\nBake everything for about 18 minutes until the salmon flakes easily and the asparagus is tender.', 15, 18, 4, '2025-05-20 00:13:03', '2025-05-20 00:36:00', '/Recipes/baked_salmon_crust.jpeg', 449, 1),
(10, 'Mom\'s Spaghetti Bolognese', 'A hearty Italian classic passed down through generations. This Bolognese is rich with flavor, loaded with vegetables, beef, and slow-simmered tomato sauce.', 'Fill a large pot with salted water and bring to a boil. Cook the spaghetti until al dente and drain.\nHeat olive oil and cook bacon until crispy. Add onion, celery, carrot, and oregano; sauté until softened.\nStir in garlic and cook until fragrant. Add ground beef and cook until browned.\nAdd balsamic vinegar and simmer until evaporated. Stir in crushed tomatoes, tomato paste, and sugar. Season with salt and pepper. Let simmer briefly, then stir in basil.\nServe the sauce over the spaghetti and top with Parmesan cheese.', 20, 40, 8, '2025-05-20 00:16:05', '2025-05-20 00:36:13', '/Recipes/spaghetti_bolognese.jpeg', 451, 1),
(11, 'N\'Awlins Stuffed Bell Peppers', 'Classic New Orleans-style stuffed bell peppers with beef, ham, and shrimp, full of Cajun flavor.', 'Preheat oven to 325°F (165°C).\nHeat oil in a large pot over low heat.\nSauté onion, celery, and chopped green pepper for 5 minutes.\nStir in garlic and cook 2 minutes.\nSeason with parsley, Creole seasoning, file powder, salt, and pepper.\nIncrease heat to medium-high and add ground beef; cook until browned.\nAdd chopped ham and cook for 5 minutes.\nStir in shrimp and cook 2 minutes more.\nRemove from heat and stir in stuffing mix.\nStuff each pepper half, sprinkle with breadcrumbs, and place in a baking dish.\nBake for 1 hour.', 25, 90, 12, '2025-05-20 00:24:11', '2025-05-20 00:36:19', '/Recipes/StuffedBellPeppers.jpeg', 498, 1),
(12, 'Easy Grilled Teriyaki Chicken', 'Juicy grilled teriyaki chicken marinated in garlic, lemon, sesame oil, and teriyaki sauce. Perfect for summer grilling.', 'Whisk teriyaki sauce, lemon juice, garlic, and sesame oil together in a bowl.\nPour marinade into a resealable bag. Add chicken, coat, and refrigerate at least 1 hour.\nPreheat grill to high heat and oil the grate.\nRemove chicken from marinade and grill 6–8 minutes per side until cooked through.\nServe hot.', 15, 15, 4, '2025-05-19 22:25:35', '2025-05-20 00:36:08', '/Recipes/GrilledTeriyakiChicken.jpeg', 240, 1),
(13, 'Flank Steak Tacos with Mango-Avocado Salsa', 'Juicy marinated flank steak tacos served with a zesty mango-avocado salsa, perfect for summer nights.', 'Combine soy sauce, olive oil, lime juice, vinegar, garlic, salt, black pepper, white pepper, garlic powder, oregano, cayenne, cumin, and paprika in a container.\nPlace flank steak in the marinade and refrigerate for 8 to 24 hours.\nMix mango, avocado, lime juice, jalapeno, red onion, and cilantro together to make the salsa.\nPreheat oven broiler, set rack 6 inches from heat.\nRemove steak from marinade and pat dry.\nBroil steak on high, flipping halfway, for about 18 minutes until slightly pink inside.\nCut into small cubes.\nWarm tortillas in microwave.\nServe steak on tortillas with salsa.', 10, 20, 8, '2025-05-20 10:24:09', '2025-05-20 14:39:25', '/Recipes/FlankSteakTacos.jpeg', 301, 1),
(14, 'Simple BBQ Ribs', 'Tender, flavorful country-style pork ribs boiled in spices then baked with BBQ sauce to perfection.', 'Place ribs in a large pot and cover with water. Stir in kosher salt, garlic powder, and pepper, and bring water to a boil over medium heat. Continue to boil until ribs are tender, 40 to 45 minutes.\r\nPreheat oven to 325°F (165°C). Remove ribs from pot and place them in a baking dish. Pour barbecue sauce over ribs and cover dish with foil.\r\nBake for 1 to 1.5 hours until internal temperature reaches 160°F (70°C). Serve hot and enjoy.', 5, 105, 4, '2025-05-20 10:26:56', '2025-05-20 14:36:40', '/Recipes/BBQRibs.jpg', 441, 1),
(15, 'Chicken Parmesan', 'Crispy, cheesy chicken Parmesan with a flavorful crust and just the right amount of sauce.', 'Preheat oven to 450°F (230°C).\r\nPlace chicken breasts between two sheets of heavy plastic and pound to 1/2-inch thickness.\r\nSeason with salt and pepper, then sprinkle with flour on both sides.\r\nBeat eggs in a bowl and set aside. Mix bread crumbs and half of the Parmesan in another bowl.\r\nDip chicken in eggs, then in bread crumb mixture. Let rest 10–15 minutes.\r\nHeat olive oil in a skillet and fry chicken until golden, about 2 minutes per side.\r\nTransfer chicken to a baking dish and top with tomato sauce, mozzarella, basil, provolone, and Parmesan.\r\nDrizzle with olive oil and bake until cheese is bubbly and chicken is cooked through, 15–20 minutes.', 15, 20, 4, '2025-05-20 10:49:24', '2025-05-20 14:36:45', '/Recipes/ChickenParmesan.jpg', 471, 1),
(16, 'Best Jambalaya', 'A spicy, one-pot Cajun dish with chicken, sausage, rice, and bold seasonings. Perfect for hearty dinners.', 'Heat 1 tablespoon peanut oil in a Dutch oven over medium heat. Season sausage and chicken with Cajun seasoning. Sauté sausage until browned and set aside.\r\nAdd another tablespoon of oil, then sauté chicken until lightly browned and set aside.\r\nIn the same pot, cook onion, bell pepper, celery, and garlic until tender.\r\nStir in crushed tomatoes, red pepper flakes, black pepper, salt, hot pepper sauce, Worcestershire sauce, and file powder.\r\nAdd chicken and sausage back to the pot. Cook for 10 minutes, stirring occasionally.\r\nStir in rice and chicken broth. Bring to a boil, then reduce heat and simmer for 20–25 minutes until liquid is absorbed.\r\nServe hot and enjoy!', 20, 45, 6, '2025-05-20 10:51:09', '2025-05-20 14:39:49', '/Recipes/Jambalaya.jpeg', 465, 1),
(17, 'Sloppy Joe Shepherd’s Pie', 'A mashup of Sloppy Joe and shepherd’s pie — savory ground beef in tangy sauce, topped with creamy mashed potatoes and melted cheese.', 'Preheat oven to 375°F (190°C). Heat olive oil in a cast-iron skillet over medium-high heat. Add garlic and cook until fragrant. Add ground chuck and cook until browned. Drain excess drippings.\r\nAdd onion and cook until softened. Stir in bouillon, brown sugar, mustard, and black pepper. Mix in crushed tomatoes, ketchup, broth, tomato paste, and Worcestershire sauce. Simmer until thickened slightly.\r\nSpread mashed potatoes over the meat mixture in an even layer. Top with shredded cheddar and Monterey Jack cheese.\r\nBake for 10 minutes until heated through and cheese melts. Broil 4–5 minutes until lightly browned. Let stand 5 minutes. Garnish with chopped chives and serve.', 15, 30, 6, '2025-05-20 10:52:32', '2025-05-20 14:36:55', '/Recipes/SloppyJoeShepherdsPie.jpg', 735, 1),
(18, 'Ground Beef and Broccoli Stir Fry', 'A quick and easy stir fry featuring ground beef, fresh broccoli, and a savory-sweet sauce, perfect for busy weeknights.', 'Whisk together beef broth, soy sauce, brown sugar, cornstarch, ginger, garlic, and rice vinegar in a bowl until sugar and cornstarch dissolve; set aside.\r\nHeat 2 tablespoons oil in a skillet over medium-high. Add onion and cook until softened, about 2 minutes. Add broccoli and cook until bright green and tender-crisp, 6 to 7 minutes. Transfer to a plate.\r\nAdd remaining 1 tablespoon oil to skillet. Add ground beef and salt. Cook until browned and cooked through, about 5 minutes.\r\nRe-whisk broth mixture and pour into skillet. Cook until sauce thickens slightly, 3 to 5 minutes. Return broccoli and onion to skillet and cook until coated in sauce, 3 to 4 minutes.\r\nServe over cooked jasmine or sticky rice. Garnish with scallions and sesame seeds.', 5, 20, 4, '2025-05-20 10:53:37', '2025-05-20 14:43:22', '/Recipes/GroundBeefBroccoliStirFry.jpeg', 665, 1),
(19, 'Easy Baked Pork Chops', 'Juicy, breaded pork chops baked in a creamy mushroom sauce. Perfect over mashed potatoes or rice.', 'Preheat oven to 350°F (175°C).\r\nSeason pork chops with garlic powder and seasoning salt. Dredge in flour, dip in beaten eggs, and coat with breadcrumbs.\r\nHeat oil in a skillet and brown pork chops on both sides, about 5 minutes per side.\r\nTransfer to a baking dish and cover with foil. Bake for 1 hour.\r\nMix cream of mushroom soup, milk, and white wine. Pour over pork chops.\r\nCover again and bake for an additional 30 minutes. Serve hot and enjoy.', 20, 90, 6, '2025-05-20 10:55:44', '2025-05-20 14:36:59', '/Recipes/BakedPorkChops.jpg', 457, 1),
(20, 'Lorraine\'s Club Sandwich', 'A quick and classic club sandwich layered with turkey, bacon, lettuce, tomato, and mayo.', 'Place bacon in a skillet and cook over medium-high heat until evenly browned. Drain on paper towels.\r\nSpread mayonnaise on each slice of toast.\r\nLayer turkey and lettuce on the first slice. Add a second slice of toast, then bacon and tomato.\r\nTop with the final slice of toast and serve.', 5, 5, 1, '2025-05-20 10:56:37', '2025-05-20 14:39:06', '/Recipes/ClubSandwich.jpeg', 818, 1),
(21, 'Shrimp Scampi with Pasta', 'Classic shrimp scampi tossed with linguine in a buttery garlic white wine sauce, perfect for any dinner occasion.', 'Bring a large pot of salted water to a boil; cook linguine until nearly tender, 6 to 8 minutes. Drain.\r\nMelt 2 tablespoons butter with 2 tablespoons olive oil in a skillet over medium heat.\r\nCook and stir shallots, garlic, and red pepper flakes until shallots are translucent, 3 to 4 minutes.\r\nSeason shrimp with salt and pepper; add to skillet and cook until pink, 2 to 3 minutes. Remove and keep warm.\r\nPour in white wine and lemon juice; bring to a boil and scrape browned bits.\r\nStir in 2 tablespoons butter and 2 tablespoons olive oil; bring to a simmer.\r\nToss linguine, shrimp, and parsley into skillet until coated. Season to taste and drizzle with olive oil before serving.', 20, 20, 6, '2025-05-20 11:00:04', '2025-05-20 14:37:06', '/Recipes/ShrimpScampi.jpg', 511, 1),
(22, 'Easy and Fast Cajun Chicken Caesar Salad', 'This Cajun-spiced blackened chicken Caesar salad is an easy and flavorful meal with bacon, Parmesan, and romaine lettuce.', 'Place bacon in a large skillet and cook over medium-high heat until evenly browned; crumble and set aside.\r\nIn a preheated skillet, add chicken, Cajun seasoning, and olive oil. Cook until chicken is golden brown and cooked through; remove from heat.\r\nIn a salad bowl, toss romaine lettuce with Caesar dressing, grated Parmesan, and bacon. Top with cooked chicken strips and serve.', 15, 35, 4, '2025-05-20 11:01:00', '2025-05-20 14:37:10', '/Recipes/CajunChickenCaesarSalad.jpg', 376, 1),
(23, 'Fish Tacos', 'Crispy beer-battered cod tacos served with a homemade citrusy white sauce and shredded cabbage on warm corn tortillas.', 'To make the beer batter: Combine flour, cornstarch, baking powder, and salt in a large bowl. Blend beer and egg in a separate bowl, then stir into flour mixture until just combined.\r\nTo make the sauce: Mix yogurt and mayonnaise in a bowl; add lime juice until runny. Stir in jalapeño, capers, cayenne, oregano, cumin, and dill.\r\nFor tacos: Heat oil to 375°F (190°C). Dust cod with flour, then dip into batter. Fry until golden and drain. Lightly fry tortillas. Assemble tacos with fish, cabbage, and sauce.', 40, 20, 8, '2025-05-20 11:02:16', '2025-05-20 14:37:21', '/Recipes/FishTacos.jpg', 409, 1),
(24, 'Roast Beef Burritos', 'Turn your leftover roast beef into an amazing Mexican-style burrito with tomatoes, chilies, cheese, and lettuce.', 'Heat oil in a skillet over medium-high heat. Stir in onion and garlic; cook until tender and transparent, about 5 minutes.\r\nMix in tomatoes, roast beef, taco sauce, chile peppers, cumin, and red pepper flakes. Bring mixture to a boil. Reduce heat to medium and simmer uncovered for 25 minutes, or until thickened.\r\nTo assemble: Lay tortillas flat and add about 2/3 cup of beef mixture in the center. Sprinkle with cheese and lettuce. Fold over sides and roll to enclose the filling.', 20, 30, 6, '2025-05-20 11:03:31', '2025-05-20 14:37:26', '/Recipes/RoastBeefBurritos.jpg', 405, 1),
(25, 'Mongolian Beef and Spring Onions', 'Chinese-style Mongolian beef stir-fried with green onions in a savory soy-based sauce, best served over rice or noodles.', 'Heat 2 teaspoons of vegetable oil in a saucepan over medium heat. Add garlic and ginger; cook and stir until fragrant, about 30 seconds.\r\nStir in brown sugar, soy sauce, and water. Increase heat to medium-high and cook until the sauce boils and slightly thickens, about 4 minutes. Remove from heat and set aside.\r\nCoat flank steak slices in cornstarch and let sit until absorbed, about 10 minutes.\r\nHeat remaining vegetable oil in a skillet to 375°F (190°C). Shake off excess cornstarch and fry beef in batches until crisp, about 2 minutes. Drain on paper towels.\r\nDiscard excess oil. Return beef to the skillet, add sauce and green onions, and stir. Boil for 1 to 2 minutes until onions are bright green and slightly tender.\r\nServe hot over rice.', 15, 10, 4, '2025-05-20 11:06:12', '2025-05-20 14:40:17', '/Recipes/MongolianBeef.jpg', 391, 1),
(26, 'Sheet Pan Chicken Fajitas', 'Quick and flavorful chicken fajitas baked on a sheet pan with bell peppers, onions, and fajita spices — perfect for weeknights or feeding a crowd.', 'Combine oil, chili powder, oregano, garlic powder, onion powder, cumin, salt, black pepper, and cayenne pepper in a large resealable plastic bag. Add chicken tenders, bell peppers, and onion; seal the bag and shake to mix. Marinate in the refrigerator, 30 minutes to 2 hours.\r\nPreheat the oven to 400°F (200°C). Line a rimmed sheet pan with foil.\r\nSpread chicken mixture onto the pan and roast, stirring halfway, until the chicken is cooked through and peppers are softened, 15 to 20 minutes.\r\nSprinkle cilantro and lime juice over the mixture, stir to distribute, and serve warm.', 20, 15, 8, '2025-05-20 11:07:03', '2025-05-20 14:37:35', '/Recipes/SheetPanChickenFajitas.jpg', 200, 1),
(27, 'Sweet Lamb Curry', 'A mild and fragrant lamb curry with apples and raisins. Sweet, savory, and perfect with rice and sambals like chutney or coconut.', 'Place flour in a resealable plastic bag and season with salt and pepper. Add lamb to the bag and shake until coated.\r\nMelt 3 tablespoons of butter in a large pot over medium-high heat. Brown lamb in batches until golden and set aside.\r\nReduce heat to medium and add remaining butter. Sauté onions until soft. Add apple, chicken stock, raisins, brown sugar, curry powder, and the browned lamb. Bring to a boil.\r\nReduce heat to medium-low, cover, and simmer for 1 to 1.5 hours until lamb is tender. Stir in lemon juice and cook 2 more minutes before serving.', 30, 90, 6, '2025-05-20 11:07:52', '2025-05-20 14:37:51', '/Recipes/SweetLambCurry.jpg', 577, 1),
(28, 'Easy Paella', 'Classic Spanish paella with saffron rice, marinated chicken, chorizo, shrimp, and vibrant Mediterranean flavor.', 'Mix olive oil, paprika, oregano, salt, and pepper for the marinade in a bowl. Add chicken and coat evenly; refrigerate until needed.\r\nHeat 2 tablespoons of olive oil in a large pan over medium heat. Stir in garlic and red pepper flakes. Add rice and cook until coated in oil.\r\nStir in saffron, bay leaf, parsley, chicken stock, and lemon zest. Bring to a boil, reduce heat, cover, and simmer for 20 minutes.\r\nMeanwhile, cook marinated chicken in another pan with olive oil. Add onion and cook until translucent. Add bell pepper and chorizo; cook until browned. Add shrimp and cook until pink and opaque.\r\nSpread rice mixture on a serving dish and top with the cooked meat and seafood. Serve and enjoy.', 30, 30, 8, '2025-05-20 11:09:09', '2025-05-20 14:37:55', '/Recipes/EasyPaella.jpg', 736, 1),
(29, 'Chicken and Waffles', 'Crispy fried chicken tenders served between cheesy waffles with maple mayo, bacon, and a spicy kick — the perfect sweet and savory sandwich.', 'Whisk eggs, cream, cayenne, salt, and black pepper in a bowl. Shake flour, cornstarch, and salt together in a paper bag.\r\nDip chicken into the egg mixture, coat in the flour mixture, and let rest on a rack for 20 minutes.\r\nHeat oil in a deep fryer to 375°F (190°C). Fry chicken in small batches until golden brown, 5 to 8 minutes. Drain on paper towels.\r\nIn a bowl, mix mayonnaise, maple syrup, horseradish, and mustard powder to make maple mayo.\r\nCook bacon in a skillet until crispy, then drain on paper towels.\r\nBroil 4 waffles with 2 chicken tenders, 3 bacon slices, and 2 cheese slices on each until cheese melts, 3 to 5 minutes.\r\nSpread maple mayo on remaining waffles and place on top to form sandwiches.', 15, 30, 4, '2025-05-20 11:10:25', '2025-05-20 14:37:59', '/Recipes/ChickenWaffles.jpg', 1793, 1);

-- --------------------------------------------------------

--
-- Table structure for table `shoppinglistitems`
--

CREATE TABLE `shoppinglistitems` (
  `ShoppingListItemID` int(11) NOT NULL,
  `ShoppingListID` int(11) DEFAULT NULL,
  `IngredientID` int(11) DEFAULT NULL,
  `Quantity` decimal(8,2) DEFAULT NULL,
  `Unit` varchar(20) DEFAULT NULL,
  `IsChecked` tinyint(1) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `shoppinglists`
--

CREATE TABLE `shoppinglists` (
  `ShoppingListID` int(11) NOT NULL,
  `UserID` int(11) DEFAULT NULL,
  `Name` varchar(100) NOT NULL,
  `CreatedAt` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `userallergies`
--

CREATE TABLE `userallergies` (
  `ID` int(11) NOT NULL,
  `UserID` int(11) DEFAULT NULL,
  `AllergyID` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `userallergies`
--

INSERT INTO `userallergies` (`ID`, `UserID`, `AllergyID`) VALUES
(137, 45, 1),
(138, 45, 2),
(139, 45, 3),
(140, 45, 4),
(141, 45, 5),
(146, 46, 1),
(147, 46, 2);

-- --------------------------------------------------------

--
-- Table structure for table `userprofiles`
--

CREATE TABLE `userprofiles` (
  `UserID` int(11) NOT NULL,
  `DietType` varchar(50) DEFAULT NULL,
  `ActivityLevel` varchar(100) DEFAULT NULL,
  `Height` float DEFAULT NULL,
  `Weight` float DEFAULT NULL,
  `IsMetric` tinyint(1) DEFAULT 1,
  `FullName` varchar(255) DEFAULT NULL,
  `Age` int(11) DEFAULT NULL,
  `Gender` varchar(10) DEFAULT NULL,
  `DietGoal` enum('Gain weight','Maintain weight','Lose weight') DEFAULT NULL,
  `ProfileImagePath` varchar(255) DEFAULT NULL,
  `IsVerified` bit(1) NOT NULL DEFAULT b'0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `userprofiles`
--

INSERT INTO `userprofiles` (`UserID`, `DietType`, `ActivityLevel`, `Height`, `Weight`, `IsMetric`, `FullName`, `Age`, `Gender`, `DietGoal`, `ProfileImagePath`, `IsVerified`) VALUES
(45, 'Vegan', 'Sedentary (little or no exercise)', 188, 71, 1, 'dadwadwa', 25, 'Male', NULL, 'UserImages/defaultpicture.png', b'0'),
(46, 'Omnivore', 'Very active (hard exercise 6-7 days/week)', 188, 71, 1, 'dawdadw', 25, 'Male', NULL, 'UserImages/defaultpicture.png', b'0'),
(47, 'Omnivore', 'Extra active (very hard exercise and physical job)', 188, 71, 1, 'Jonas Imbrechts', 21, 'Male', 'Gain weight', 'UserImages/defaultpicture.png', b'0'),
(62, 'Omnivore', 'Lightly active (light exercise 1-3 days/week)', 188, 71, 1, 'Jonas', 25, 'Male', 'Maintain weight', 'UserImages/defaultpicture.png', b'0');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `UserID` int(11) NOT NULL,
  `Username` varchar(50) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `PasswordHash` varchar(64) NOT NULL,
  `CreatedAt` timestamp NOT NULL DEFAULT current_timestamp(),
  `UpdatedAt` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `Role` varchar(20) NOT NULL DEFAULT 'User'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`UserID`, `Username`, `Email`, `PasswordHash`, `CreatedAt`, `UpdatedAt`, `Role`) VALUES
(45, 'AdminAdmin', 'adminadmin@hotmail.com', 'RPQroeX69d0gr5ggmuGMGqGnT+JmknoX579ZrEOoTOA=', '2025-05-20 13:19:53', '2025-05-20 13:19:53', 'Admin'),
(46, 'Pedro', 'pedro@hotmail.com', 'RPQroeX69d0gr5ggmuGMGqGnT+JmknoX579ZrEOoTOA=', '2025-05-20 13:40:11', '2025-05-20 13:40:11', 'Admin'),
(47, 'Jonas', 'jonasimbrechts123@hotmail.com', 'RPQroeX69d0gr5ggmuGMGqGnT+JmknoX579ZrEOoTOA=', '2025-05-20 14:12:10', '2025-05-20 14:12:10', 'User'),
(62, 'Admin', 'jonasimbrechts12@hotmail.com', 'RPQroeX69d0gr5ggmuGMGqGnT+JmknoX579ZrEOoTOA=', '2025-05-21 10:30:42', '2025-05-21 10:30:42', 'Admin');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `allergies`
--
ALTER TABLE `allergies`
  ADD PRIMARY KEY (`AllergyID`);

--
-- Indexes for table `categories`
--
ALTER TABLE `categories`
  ADD PRIMARY KEY (`CategoryID`),
  ADD UNIQUE KEY `Name` (`Name`);

--
-- Indexes for table `emailsettings`
--
ALTER TABLE `emailsettings`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `favorites`
--
ALTER TABLE `favorites`
  ADD PRIMARY KEY (`UserID`,`RecipeID`),
  ADD KEY `RecipeID` (`RecipeID`);

--
-- Indexes for table `ingredients`
--
ALTER TABLE `ingredients`
  ADD PRIMARY KEY (`IngredientID`),
  ADD UNIQUE KEY `Name` (`Name`),
  ADD UNIQUE KEY `Name_2` (`Name`);

--
-- Indexes for table `mealplanitems`
--
ALTER TABLE `mealplanitems`
  ADD PRIMARY KEY (`MealPlanItemID`),
  ADD KEY `MealPlanID` (`MealPlanID`),
  ADD KEY `RecipeID` (`RecipeID`);

--
-- Indexes for table `mealplans`
--
ALTER TABLE `mealplans`
  ADD PRIMARY KEY (`MealPlanID`),
  ADD KEY `UserID` (`UserID`);

--
-- Indexes for table `recipeallergies`
--
ALTER TABLE `recipeallergies`
  ADD PRIMARY KEY (`RecipeID`,`AllergyID`),
  ADD KEY `AllergyID` (`AllergyID`);

--
-- Indexes for table `recipecategories`
--
ALTER TABLE `recipecategories`
  ADD PRIMARY KEY (`RecipeID`,`CategoryID`),
  ADD KEY `CategoryID` (`CategoryID`);

--
-- Indexes for table `recipeingredients`
--
ALTER TABLE `recipeingredients`
  ADD PRIMARY KEY (`RecipeID`,`IngredientID`),
  ADD KEY `IngredientID` (`IngredientID`);

--
-- Indexes for table `recipes`
--
ALTER TABLE `recipes`
  ADD PRIMARY KEY (`RecipeID`),
  ADD KEY `FK_Recipes_Categories` (`CategoryID`);

--
-- Indexes for table `shoppinglistitems`
--
ALTER TABLE `shoppinglistitems`
  ADD PRIMARY KEY (`ShoppingListItemID`),
  ADD KEY `ShoppingListID` (`ShoppingListID`),
  ADD KEY `IngredientID` (`IngredientID`);

--
-- Indexes for table `shoppinglists`
--
ALTER TABLE `shoppinglists`
  ADD PRIMARY KEY (`ShoppingListID`),
  ADD KEY `UserID` (`UserID`);

--
-- Indexes for table `userallergies`
--
ALTER TABLE `userallergies`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `UserID` (`UserID`),
  ADD KEY `FK_UserAllergies_AllergyID` (`AllergyID`);

--
-- Indexes for table `userprofiles`
--
ALTER TABLE `userprofiles`
  ADD PRIMARY KEY (`UserID`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`UserID`),
  ADD UNIQUE KEY `Username` (`Username`),
  ADD UNIQUE KEY `Email` (`Email`),
  ADD KEY `idx_username_email` (`Username`,`Email`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `allergies`
--
ALTER TABLE `allergies`
  MODIFY `AllergyID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `categories`
--
ALTER TABLE `categories`
  MODIFY `CategoryID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `emailsettings`
--
ALTER TABLE `emailsettings`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `ingredients`
--
ALTER TABLE `ingredients`
  MODIFY `IngredientID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=308;

--
-- AUTO_INCREMENT for table `mealplanitems`
--
ALTER TABLE `mealplanitems`
  MODIFY `MealPlanItemID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=134;

--
-- AUTO_INCREMENT for table `mealplans`
--
ALTER TABLE `mealplans`
  MODIFY `MealPlanID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT for table `recipes`
--
ALTER TABLE `recipes`
  MODIFY `RecipeID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=30;

--
-- AUTO_INCREMENT for table `shoppinglistitems`
--
ALTER TABLE `shoppinglistitems`
  MODIFY `ShoppingListItemID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `shoppinglists`
--
ALTER TABLE `shoppinglists`
  MODIFY `ShoppingListID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `userallergies`
--
ALTER TABLE `userallergies`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=163;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `UserID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=63;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `favorites`
--
ALTER TABLE `favorites`
  ADD CONSTRAINT `favorites_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE,
  ADD CONSTRAINT `favorites_ibfk_2` FOREIGN KEY (`RecipeID`) REFERENCES `recipes` (`RecipeID`) ON DELETE CASCADE;

--
-- Constraints for table `mealplanitems`
--
ALTER TABLE `mealplanitems`
  ADD CONSTRAINT `mealplanitems_ibfk_1` FOREIGN KEY (`MealPlanID`) REFERENCES `mealplans` (`MealPlanID`) ON DELETE CASCADE,
  ADD CONSTRAINT `mealplanitems_ibfk_2` FOREIGN KEY (`RecipeID`) REFERENCES `recipes` (`RecipeID`) ON DELETE CASCADE;

--
-- Constraints for table `mealplans`
--
ALTER TABLE `mealplans`
  ADD CONSTRAINT `mealplans_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE;

--
-- Constraints for table `recipeallergies`
--
ALTER TABLE `recipeallergies`
  ADD CONSTRAINT `recipeallergies_ibfk_1` FOREIGN KEY (`RecipeID`) REFERENCES `recipes` (`RecipeID`) ON DELETE CASCADE,
  ADD CONSTRAINT `recipeallergies_ibfk_2` FOREIGN KEY (`AllergyID`) REFERENCES `allergies` (`AllergyID`) ON DELETE CASCADE;

--
-- Constraints for table `recipecategories`
--
ALTER TABLE `recipecategories`
  ADD CONSTRAINT `recipecategories_ibfk_1` FOREIGN KEY (`RecipeID`) REFERENCES `recipes` (`RecipeID`) ON DELETE CASCADE,
  ADD CONSTRAINT `recipecategories_ibfk_2` FOREIGN KEY (`CategoryID`) REFERENCES `categories` (`CategoryID`) ON DELETE CASCADE;

--
-- Constraints for table `recipeingredients`
--
ALTER TABLE `recipeingredients`
  ADD CONSTRAINT `recipeingredients_ibfk_1` FOREIGN KEY (`RecipeID`) REFERENCES `recipes` (`RecipeID`) ON DELETE CASCADE,
  ADD CONSTRAINT `recipeingredients_ibfk_2` FOREIGN KEY (`IngredientID`) REFERENCES `ingredients` (`IngredientID`) ON DELETE CASCADE;

--
-- Constraints for table `recipes`
--
ALTER TABLE `recipes`
  ADD CONSTRAINT `FK_Recipes_Categories` FOREIGN KEY (`CategoryID`) REFERENCES `categories` (`CategoryID`) ON DELETE SET NULL;

--
-- Constraints for table `shoppinglistitems`
--
ALTER TABLE `shoppinglistitems`
  ADD CONSTRAINT `shoppinglistitems_ibfk_1` FOREIGN KEY (`ShoppingListID`) REFERENCES `shoppinglists` (`ShoppingListID`) ON DELETE CASCADE,
  ADD CONSTRAINT `shoppinglistitems_ibfk_2` FOREIGN KEY (`IngredientID`) REFERENCES `ingredients` (`IngredientID`) ON DELETE CASCADE;

--
-- Constraints for table `shoppinglists`
--
ALTER TABLE `shoppinglists`
  ADD CONSTRAINT `shoppinglists_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`) ON DELETE CASCADE;

--
-- Constraints for table `userallergies`
--
ALTER TABLE `userallergies`
  ADD CONSTRAINT `FK_UserAllergies_AllergyID` FOREIGN KEY (`AllergyID`) REFERENCES `allergies` (`AllergyID`) ON DELETE CASCADE,
  ADD CONSTRAINT `userallergies_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`);

--
-- Constraints for table `userprofiles`
--
ALTER TABLE `userprofiles`
  ADD CONSTRAINT `userprofiles_ibfk_1` FOREIGN KEY (`UserID`) REFERENCES `users` (`UserID`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
