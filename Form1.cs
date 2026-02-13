using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace NewsParser
{
    public partial class MainForm : Form
    {
        private string currentCategoryUrl = "https://ria.ru/world/";

        public MainForm()
        {
            
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Настройка формы
            this.Text = "Парсер новостей";
            this.BackColor = Color.FromArgb(45, 45, 45);
            this.Size = new Size(1980, 1080);

            // Панель для кнопок категорий
            var categoryPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(30, 30, 30),
                Padding = new Padding(5),
                AutoSize = true,
            };

            // Кнопки категорий
            var worldButton = CreateCategoryButton("Мир", "https://ria.ru/world/");
            var scienceButton = CreateCategoryButton("Наука", "https://ria.ru/science/");
            var economyButton = CreateCategoryButton("Экономика", "https://ria.ru/economy/");
            var societyButton = CreateCategoryButton("Общество", "https://ria.ru/society/");
            var incidentsButton = CreateCategoryButton("Инциденты", "https://ria.ru/incidents/");

            categoryPanel.Controls.Add(worldButton);
            categoryPanel.Controls.Add(scienceButton);
            categoryPanel.Controls.Add(economyButton);
            categoryPanel.Controls.Add(societyButton);
            categoryPanel.Controls.Add(incidentsButton);

            // Панель для новостей
            newsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 45, 45),
                AutoScroll = true,
                Padding = new Padding(10),
            };

            var settingsButton = new Button
            {
                Text = "⚙",
                Width = 40,
                Height = 40,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = new Font("Arial", 12, FontStyle.Bold),
            };
            settingsButton.FlatAppearance.BorderSize = 0;
            settingsButton.Click += SettingsButton_Click;

            // Добавление элементов на форму
            this.Controls.Add(settingsButton);
            this.Controls.Add(newsPanel);
            this.Controls.Add(categoryPanel);

            // Установка позиции кнопки настроек
            settingsButton.Location = new Point(this.Width - settingsButton.Width - 20, 10);
            this.Resize += (s, e) =>
            {
                settingsButton.Location = new Point(this.Width - settingsButton.Width - 20, 10);
            };
            // Загрузка новостей по умолчанию
            _ = LoadNews();
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm(this);
            settingsForm.ShowDialog();
        }

        public void UpdateCategoryUrl(string url)
        {
            currentCategoryUrl = url;
            if (currentCategoryUrl == "https://belta.by/economics/")
            {
                _ = LoadNewsRB();
            }
            else
            {
                _ = LoadNews(); // Загружаем новости с новой категории
            }
        }

        private Button CreateCategoryButton(string text, string url)
        {
            var button = new Button
            {
                Text = text,
                Width = 100,
                Height = 40,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Tag = url
            };

            button.FlatAppearance.BorderSize = 0;
            button.Click += (s, e) =>
            {
                currentCategoryUrl = (string)button.Tag;
                _ = LoadNews();
            };

            return button;
        }

        private async Task LoadNews()
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true
            };
            var httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

            try
            {
                // Очищаем панель перед загрузкой новостей
                newsPanel.Controls.Clear();

                // Загрузка HTML
                string html = await httpClient.GetStringAsync(currentCategoryUrl);
                if (string.IsNullOrEmpty(html))
                {
                    MessageBox.Show("HTML не загружен!");
                    return;
                }

                // Парсинг HTML
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Проверка структуры новостей
                var newsItems = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'list-item')]");
                if (newsItems == null || newsItems.Count == 0)
                {
                    MessageBox.Show("Новости не найдены. Проверьте структуру сайта.");
                    return;
                }

                // Хранение уникальных ссылок на новости
                var addedLinks = new HashSet<string>();

                foreach (var item in newsItems)
                {
                    // Извлечение заголовка и ссылки
                    var titleNode = item.SelectSingleNode(".//a[contains(@class, 'list-item__title')]");
                    var link = titleNode?.GetAttributeValue("href", string.Empty);
                    var title = titleNode?.InnerText.Trim();

                    // Проверка на уникальность ссылки и корректность данных
                    if (!string.IsNullOrEmpty(link) && !string.IsNullOrEmpty(title) && !addedLinks.Contains(link))
                    {
                        addedLinks.Add(link); // Добавляем ссылку в HashSet, чтобы исключить дубликаты

                        // Создание кнопки для новости
                        var newsButton = new Button
                        {
                            Text = title,
                            Width = newsPanel.Width - 40, // Учитываем отступы панели
                            Height = 50,                 // Фиксированная высота кнопки
                            BackColor = Color.FromArgb(60, 60, 60),
                            ForeColor = Color.White,
                            Tag = link,
                            Margin = new Padding(5)     // Отступы между кнопками
                        };

                        // Добавляем обработчик нажатия
                        newsButton.Click += NewsButton_Click;

                        // Добавляем кнопку в панель
                        newsPanel.Controls.Add(newsButton);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private async Task LoadNewsRB()
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true
            };
            var httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

            try
            {
                // Очищаем панель перед загрузкой новостей
                newsPanel.Controls.Clear();

                // Загрузка HTML
                string html = await httpClient.GetStringAsync(currentCategoryUrl);
                if (string.IsNullOrEmpty(html))
                {
                    MessageBox.Show("HTML не загружен!");
                    return;
                }

                // Парсинг HTML
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Проверка структуры новостей
                var newsItems = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'news_item')]");
                if (newsItems == null || newsItems.Count == 0)
                {
                    MessageBox.Show("Новости не найдены. Проверьте структуру сайта.");
                    return;
                }
                else
                {
                    MessageBox.Show(newsItems.Count.ToString());
                }
                // Хранение уникальных ссылок на новости
                var addedLinks = new HashSet<string>();

                foreach (var item in newsItems)
                {
                    // Извлечение заголовка и ссылки
                    var titleNode = item.SelectSingleNode(".//a");
                    var link = titleNode?.GetAttributeValue("href", string.Empty);
                    var title = titleNode?.InnerText.Trim();

                    // Проверка на уникальность ссылки и корректность данных
                    if (!string.IsNullOrEmpty(link) && !string.IsNullOrEmpty(title) && !addedLinks.Contains(link))
                    {
                        if (!link.StartsWith("https://"))
                        {
                            link = "https://belta.by" + link;
                        }

                        addedLinks.Add(link); // Добавляем ссылку в HashSet, чтобы исключить дубликаты

                        // Создание кнопки для новости
                        var newsButtonRB = new Button
                        {
                            Text = title,
                            Width = newsPanel.Width - 40, // Учитываем отступы панели
                            Height = 50,                 // Фиксированная высота кнопки
                            BackColor = Color.FromArgb(60, 60, 60),
                            ForeColor = Color.White,
                            Tag = link,
                            Margin = new Padding(5)     // Отступы между кнопками
                        };

                        // Добавляем обработчик нажатия
                        newsButtonRB.Click += NewsButtonRB_Click;

                        // Добавляем кнопку в панель
                        newsPanel.Controls.Add(newsButtonRB);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void NewsButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var link = button.Tag as string;
            if (string.IsNullOrEmpty(link)) return;

            // Открываем окно с содержанием новости
            var newsForm = new NewsDetailsForm(link);
            newsForm.ShowDialog();
        }

        private void NewsButtonRB_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var link = button.Tag as string;
            if (string.IsNullOrEmpty(link)) return;

            // Открываем окно с содержанием новости
            var newsForm = new NewsDetailsFormRB(link);
            newsForm.ShowDialog();
        }

        private FlowLayoutPanel newsPanel;
    }

    public class SettingsForm : Form
    {
        private readonly MainForm mainForm;

        public SettingsForm(MainForm form)
        {
            mainForm = form; // Сохраняем ссылку на главную форму
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Настройки";
            this.Size = new Size(400, 300);
            this.BackColor = Color.FromArgb(45, 45, 45);

            // Панель для кнопок
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 45, 45),
                Padding = new Padding(10),
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown
            };

            // Создаем кнопки
            var russiaButton = CreateCountryButton("Россия", "https://ria.ru/world/");
            var belarusButton = CreateCountryButton("Беларусь", "https://belta.by/economics/");
            var usaButton = CreateCountryButton("США", "https://www.golosameriki.com/novosti");

            // Добавляем кнопки на панель
            buttonPanel.Controls.Add(russiaButton);
            buttonPanel.Controls.Add(belarusButton);
            buttonPanel.Controls.Add(usaButton);

            // Добавляем панель в форму
            this.Controls.Add(buttonPanel);
        }

        private Button CreateCountryButton(string countryName, string url)
        {
            var button = new Button
            {
                Text = countryName,
                Width = 150,
                Height = 40,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 12, FontStyle.Regular)
            };

            button.FlatAppearance.BorderSize = 0;

            button.Click += (s, e) =>
            {
                mainForm.UpdateCategoryUrl(url); // Изменяем категорию в главной форме
                this.Close(); // Закрываем окно настроек
            };

            return button;
        }
    }

    public class NewsDetailsForm : Form
    {
        public NewsDetailsForm(string link)
        {
            InitializeUI(link);
        }

        private async void InitializeUI(string link)
        {
            this.Text = "Новость";
            this.Size = new Size(600, 1080);
            this.BackColor = Color.FromArgb(45, 45, 45);

            // Основная панель с вертикальной прокруткой
            var contentPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(60, 60, 60),
                Padding = new Padding(10),
                AutoScroll = true
            };

            // Кнопка для открытия в браузере
            var openInBrowserButton = new Button
            {
                Text = "Открыть в браузере",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White
            };

            openInBrowserButton.Click += (s, e) =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = link,
                    UseShellExecute = true
                });
            };

            this.Controls.Add(contentPanel);
            this.Controls.Add(openInBrowserButton);

            try
            {
                // Загружаем HTML статьи
                var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(link);

                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Извлекаем заголовок новости
                var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'article__title')]");
                var title = titleNode?.InnerText.Trim() ?? "Заголовок не найден";

                var titleLabel = new Label
                {
                    Text = title,
                    ForeColor = Color.White,
                    Font = new Font("Arial", 14, FontStyle.Bold),
                    AutoSize = true,
                    MaximumSize = new Size(contentPanel.Width - 20, 0)
                };

                contentPanel.Controls.Add(titleLabel);

                // Извлекаем изображение
                var imageNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'photoview__open')]//img");
                if (imageNode != null)
                {
                    var imageUrl = imageNode.GetAttributeValue("src", null);
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        var pictureBox = new PictureBox
                        {
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            Anchor = AnchorStyles.Top | AnchorStyles.Left,
                            Width = 400,
                            Height = 350,
                            Margin = new Padding(5)
                        };

                        // Загружаем изображение
                        using (var stream = await httpClient.GetStreamAsync(imageUrl))
                        {
                            pictureBox.Image = Image.FromStream(stream);
                        }

                        contentPanel.Controls.Add(pictureBox);
                    }
                }

                // Извлекаем текст статьи
                var articleContentNode = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'article__text')]");
                var articleText = string.Join("\n\n", articleContentNode.Select(p => p.InnerText.Trim()));
                var articleTextLabel = new Label
                {
                    Text = articleText,
                    ForeColor = Color.White,
                    Font = new Font("Arial", 10),
                    AutoSize = true,
                    MaximumSize = new Size(contentPanel.Width - 20, 0)
                };

                contentPanel.Controls.Add(articleTextLabel);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить содержимое новости: {ex.Message}");
            }
        }


    }
    public class NewsDetailsFormRB : Form
    {
        public NewsDetailsFormRB(string link)
        {
            InitializeUI(link);
        }

        private async void InitializeUI(string link)
        {
            this.Text = "Новость";
            this.Size = new Size(600, 1080);
            this.BackColor = Color.FromArgb(45, 45, 45);

            // Основная панель с вертикальной прокруткой
            var contentPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(60, 60, 60),
                Padding = new Padding(10),
                AutoScroll = true
            };

            // Кнопка для открытия в браузере
            var openInBrowserButton = new Button
            {
                Text = "Открыть в браузере",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White
            };

            openInBrowserButton.Click += (s, e) =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = link,
                    UseShellExecute = true
                });
            };

            this.Controls.Add(contentPanel);
            this.Controls.Add(openInBrowserButton);

            try
            {
                // Загружаем HTML статьи
                var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(link);

                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Извлекаем заголовок новости
                var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//h1");
                var title = titleNode?.InnerText.Trim() ?? "Заголовок не найден";

                var titleLabel = new Label
                {
                    Text = title,
                    ForeColor = Color.White,
                    Font = new Font("Arial", 14, FontStyle.Bold),
                    AutoSize = true,
                    MaximumSize = new Size(contentPanel.Width - 20, 0)
                };

                contentPanel.Controls.Add(titleLabel);

                // Извлекаем изображение
                var imageNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'news_img_slide')]//img");
                if (imageNode != null)
                {
                    var imageUrl = imageNode.GetAttributeValue("src", null);
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        var pictureBox = new PictureBox
                        {
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            Anchor = AnchorStyles.Top | AnchorStyles.Left,
                            Width = 400,
                            Height = 350,
                            Margin = new Padding(5)
                        };

                        // Загружаем изображение
                        using (var stream = await httpClient.GetStreamAsync(imageUrl))
                        {
                            pictureBox.Image = Image.FromStream(stream);
                        }

                        contentPanel.Controls.Add(pictureBox);
                    }
                }

                // Извлекаем текст статьи
                var articleContentNode = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'js-mediator-article')]");
                var articleText = string.Join("\n\n", articleContentNode.Select(p => p.InnerText.Trim()));
                var articleTextLabel = new Label
                {
                    Text = articleText,
                    ForeColor = Color.White,
                    Font = new Font("Arial", 10),
                    AutoSize = true,
                    MaximumSize = new Size(contentPanel.Width - 20, 0)
                };

                contentPanel.Controls.Add(articleTextLabel);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить содержимое новости: {ex.Message}");
            }
        }


    }
}
