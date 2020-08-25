using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CommentsTest.Models
{
    public class CommentsRepository
    {
        readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        /// <summary>
        /// Получаем список всех статей
        /// </summary>
        /// <returns></returns>
        public List<Article> GetArticles()
        {
            List<Article> articles = new List<Article>();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                articles = db.Query<Article>("SELECT * FROM Articles").ToList();

                foreach (var article in articles)
                {
                    article.Comments = GetComments(article.ID).ToList();
                }

            }
            return articles;
        }

        /// <summary>
        /// Получаем статью по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Article GetArticle(int id)
        {
            Article article = new Article();

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                article = db.Query<Article>("SELECT * FROM Articles WHERE ID = " + id).FirstOrDefault();
                article.Comments = GetComments(id).ToList();
            }
            return article;
        }

        /// <summary>
        /// Получаем id статьи
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetArticleID(int id)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<int>(@"EXEC GetArticleID " + id).FirstOrDefault();
            }
        }

        /// <summary>
        /// Получаем список форматированный список комментов
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        public IEnumerable<Comment> GetComments(int articleId)
        {
            var comments = new Dictionary<string, Comment>();

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Query<Comment, ParentComment, Article, User, Comment>(@"EXEC GetFormatedComments " + articleId.ToString(), (comment, parentComment, article, user) =>
                {
                    var current = comment;
                    if (!comments.TryGetValue(comment.ID.ToString(), out current))
                    {
                        current = comment;
                        comments.Add(current.ID.ToString(), current);
                    }
                    current.Article = article;
                    current.User = user;
                    if (parentComment != null)
                    {
                        current.Parent = new Comment { ID = parentComment.ParentID, Text = parentComment.Text };
                    }
                    return current;
                }, splitOn: "ID, ParentID, ID, ID");
            }
            return comments.Values;
        }

        /// <summary>
        /// Получаем конкретный коммент
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Comment GetComment(int id)
        {
            var comment = new Comment();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                comment = db.Query<Comment>("SELECT * FROM Comments WHERE ID = " + id).FirstOrDefault();
            }
            return comment;
        }

        /// <summary>
        /// Добавляем новый коммент
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public Comment AddComment(Comment comment)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string parent = "NULL";
                if (comment.Parent != null)
                    parent = comment.Parent.ID.ToString();
                string sqlQuery = @"INSERT INTO Comments (Text, UserID, ArticleID, ParentID) VALUES(N'" + comment.Text + "', " + comment.User.ID + ", " + comment.Article.ID + ", " + parent + ")";
                int? commentId = db.Query<int>(sqlQuery, comment).FirstOrDefault();
                comment.ID = (int)commentId;
            }
            return comment;
        }

        /// <summary>
        /// Получаем пользователя по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetUser(int? id)
        {
            User user = new User();
            if (id == null)
                return new User { Name = "Пользователь" };
            else
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    user = db.Query<User>("SELECT * FROM Users WHERE ID = " + id).FirstOrDefault();
                }
                return user;
            }
        }

        /// <summary>
        /// Проверяем пользователя по имени
        /// Если есть имя в бд, то возвращаем его id
        /// Если нет такого - создаем запись и возвращаем id нового юзера
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public User CheckUser(string name)
        {
            User user = new User() { Name = name };
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                user = db.Query<User>("SELECT ID FROM Users WHERE Name = N'" + name + "'").FirstOrDefault();
                if (user == null)
                {
                    var sqlQuery = "INSERT INTO Users (Name) VALUES(N'" + name + "'); SELECT CAST(SCOPE_IDENTITY() as int)";
                    int userId = db.Query<int>(sqlQuery, user).FirstOrDefault();
                    return new User { ID = userId, Name = name };
                }
            }
            return user;
        }

        /// <summary>
        /// Заполнение бд тестовыми данным при каждом запуске
        /// </summary>
        public void Initialize()
        {
            #region
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var sqlQuery = "DELETE FROM Articles " +
                               "DELETE FROM Comments " +
                               "DELETE FROM Users";
                db.Execute(sqlQuery);

                sqlQuery = "INSERT INTO Users VALUES(N'Евгений') " +
                           "INSERT INTO Users VALUES(N'Василий') " +
                           "INSERT INTO Users VALUES(N'Петр') " +
                           "INSERT INTO Users VALUES(N'Аркадий') " +
                           "INSERT INTO Users VALUES(N'Иннокентий') " +
                           "INSERT INTO Users VALUES(N'Рустем') " +
                           "INSERT INTO Articles VALUES(N'Цифровизация образования', N'В школах планируется дистанционное обучение с 1 сентября 2020 Цифровизация образования в России — к чему это приведет? Во время нынешнего карантина школы и родители кое как пережили дистанционное обучение, которое длилось почти два месяца. И не пришлось долго ждать, как тут же появилась новая инициатива, которую уже восприняли в штыки. Дистанционное обучение с 1 сентября 2020 года продолжится во всех школах страны – именно так все восприняли новый проект постановления Правительства «О проведении в 2020 — 2022 годах эксперимента по внедрению целевой модели цифровой образовательной среды». Но давайте разбираться, так ли это на самом деле. Цифровая образовательная среда не равно дистанционное образование, объяснили в Министерстве просвещения. С 1 сентября все школьники сядут за парты, и для них начнётся вполне обычный учебный процесс. Тогда же и начнется эксперимент по внедрению в школах и колледжах цифровой образовательной среды. Дистанционное обучение стимулировало рост самых разных образовательных сервисов и платформ, в том числе и государственных. Во время карантина и самоизоляции школьники и педагоги пользовались ими вынужденно – альтернатив у них не было. Но коронавирусные ограничения рано или поздно закончатся, не пропадать же платформам и накопленным технологиям? Чтобы понять, как можно использовать цифровую образовательную среду в мирное время, как раз и нужен эксперимент. «Цифровая образовательная среда» (ЦОС) — один из федеральных проектов национального проекта «Образование», напоминают в Министерстве просвещения. Ожидается, что к 2024 году ЦОС будет внедрена по всей стране.') " +
                           "INSERT INTO Articles VALUES(N'Отдых в Испании', N'Испания — страна, где найти для себя возможность отлично отдохнуть сможет любой турист. Те, кто любит пляж, солнечные ванны и купания в тёплом море, замечательно проведут время в любом её уголке. Тем же, кто предпочитает прогулки по тихим улочкам и посещение интереснейших музеев с богатыми экспозициями, следует обязательно наведаться в Барселону. Здесь их ждёт Ла Саграда Фамилиа — творение Антонио Гауди, не имеющее мировых аналогов. Среди наиболее посещаемых достопримечательностей Барселоны можно также назвать ещё одно детище Гауди — Casa Mila. Его второе название — La Pedrera, или «каменоломня». Это причудливое строение с тяжеловесным фасадом, волнистыми стенами и крышей и скульптурными деталями с майоликовой облицовкой несколько напоминает каменную пещеру. В нём сегодня располагается музей, посвящённый творчеству выдающегося испанского архитектора. Барселона — сказочный город, который даёт возможность познать красоту всей Испании. Ведь именно здесь располагается Испанская деревня, или Poble Espanyol, — музей архитектуры под открытым небом. Он представляет собой своего рода город в городе и является вотчиной мастеров и ремесленников. Здесь располагается 116 домов, дворцов и церквей, а также многочисленные мастерские, хозяева которых любят обучать гостей своему ремеслу. Семейным туристам с детьми в Барселоне нужно обязательно посетить зоопарк и аквариум. Надо сказать, они располагаются совсем недалеко друг от друга, поэтому на наблюдение за животными и морскими обитателями можно провести весь день. Аквариум в Барселоне — это настоящий интерактивный развлекательный центр с 35 аквариумами. Здесь живёт около 11 тысяч различных представителей фауны Средиземного моря. Гордостью аквариума является его 80-метровый тоннель, окружённый со всех сторон водой, в которой плавают рыбы.') " +
                           "INSERT INTO Articles VALUES(N'Варшава. Главный город Польши за один день.', N'Варшава- это самый крупный город Польши, по совместительству столица. И столицей город является аж с конца XVI века, после того, как король Сигизмунд III перенес свою резиденцию из Вавельского замка в Кракове сюда. Численность населения составляет около 1,8 млн человек, а всей Польши 38,6 млн человек. В городе очень хорошо развит общественный транспорт, который работает круглосуточно. Метро, трамваи, автобусы и пригородные поезда, все в хорошем состоянии- чистое и ухоженное, как, наверное, и все в Польше. Составы в метро разные, есть и старого образца, а есть и нового. Трамваи мы видели только новые, возможно где-то ездят и старенькие. Почему пишу больше о метро и трамваях? Да потому, что они не зависят от пробок и, на мой взгляд, пользоваться ими удобней. Также Варшава является крупным европейским транспортным узлом, благодаря своему удобному расположению. Главным символом города является Варшавская русалочка - Сирена, ее изображение есть даже на гербе. И как вы уже могли догадаться, в старом городе установлен памятник в ее честь, а если быть точным, то целых два. Первый - XIX века, установлен на рыночной площади старого города, а второй находится на набережной реки Вислы, и создан он был в 1939 году. Картинка с сайта commons.m.wikimedia.org Картинка с сайта commons.m.wikimedia.org Вообще, весь старый город и основные достопримечательности расположились вдоль реки Вислы, как и у большинства старинных городов. Начать стоит с Дворцовой площади и после пойти в правую сторону (если стоять лицом к площади), как раз по направлению к реке. В самом центре площади стоит колонна Сигизмунда III, в восточной части площади расположился Королевский замок. А рядом с ним Кафедральный собор Святого Иоанна Крестителя (на польском Святого Яна), построенный в конце XIV века, и внесенный в список всемирного наследия ЮНЕСКО. Канония - небольшая улица в центре старого города, а вернее треугольная площадь. Свое название получила еще в далеком XVII веке благодаря тому, что в домах, расположенных по периметру этой площади жили каноники местного капитула. На этом месте располагалось кладбище еще до того, как были построены дома каноников. В центре площади красуется колокол, отлитый в XVII веке. Также в Старом городе находятся: Костел Иезуитов или Костел Милостивой Божьей Матери, Костел Святого Мартина, гнойная гора, памятник юному повстанцу и т.д. Да и вообще, сами детали Старого города очень красивы, будь то часы со знаками зодиака или просто фасады домов. Выходим из Старого города и попадаем к стенам Варшавского барбакана. Это полукруглый укреплённый передовой пост, одно из немногих укреплений сохранившихся с XVII века. Расположен он между новым и старым городом. Минуя его проходим в новый город. Здесь вашему вниманию представляются Костел Святого Яцека, Костел Святого Духа, Кастел Святого Казимира, Костел Посещения Пресвятой Девы Марии, Музей Марии Склодовской-Кюри, а также рынок Нового города. Поблизости расположен Мультимедийный парк фонтанов и если есть возможность, то сюда лучше приходить вечером. В период с мая по сентябрь по пятницам и субботам здесь проходят потрясающие мультимедийные шоу. К моему глубочайшему сожалению, мы на это шоу не попали, но все еще впереди! Вкратце о том что можно посмотреть в Варшаве. Удачных вам поездок! И вообще. Зачем я это копирую?')";
                db.Execute(sqlQuery);
            }
            #endregion
        }
    }
}