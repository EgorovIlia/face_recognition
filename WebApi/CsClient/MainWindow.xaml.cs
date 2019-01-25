using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using ClientExtensions;
using System.ComponentModel;

using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using CategoryEnumeration;
using System.Net.Http;

namespace CsClient //Lab2v1   version 0
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Replace <Subscription Key> with your valid subscription key.
        const string subscriptionKey = "<Subscription Key>";

        private const string faceEndpoint =
            "https://westeurope.api.cognitive.microsoft.com";

        CancellationTokenSource cts;// = new CancellationTokenSource();
        CancellationToken token;// = cts.Token;

        public static RoutedCommand Interrupt = new RoutedCommand();

        private readonly IFaceClient faceClient = new FaceClient(
            new ApiKeyServiceClientCredentials(subscriptionKey),
            new System.Net.Http.DelegatingHandler[] { });

        static CategoriesObservable categories = new CategoriesObservable();
        static HttpClient client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();

            cts = new CancellationTokenSource();
            token = cts.Token;

            if (Uri.IsWellFormedUriString(faceEndpoint, UriKind.Absolute))
            {
                faceClient.Endpoint = faceEndpoint;
            }
            else
            {
                MessageBox.Show(faceEndpoint,
                    "Invalid URI", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            categoryAmountListbox.ItemsSource = categories;
            categoryListbox.ItemsSource = categories;

            DataContext = categories; //ссылка на список;использовать как источник данных в привязках

            client.BaseAddress = new Uri("http://localhost:60632/"); //60632

        }

        private async void InitializeData()
        {
            //загрузка данных из БД
            try
            {
                var result = await client.GetAsync("api/faces");
                result.EnsureSuccessStatusCode();
                var faces = await result.Content.ReadAsAsync<Shared.Face[]>();
                categories.Clear();
                foreach (var face in faces)
                {
                    categories.AddCategoryFace(ConvertTools.ToClient(face));
                }
            }
            catch (System.Net.Http.HttpRequestException)
            {
                MessageBox.Show("Не удается установить соединение сервером", "Ошибка загрузки");
            }
        }

        private void Choose_dir_Click(object sender, RoutedEventArgs e)
        {
            string dir_path;
            using (var folderDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                //folderDialog.SelectedPath = @"";

                System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    // folderDialog.SelectedPath -- your result
                    dir_path = folderDialog.SelectedPath;

                    Computation(dir_path, token);
                }
            }
        }

        private void Computation(string dir_path, CancellationToken token)
        {

            var images = Directory.EnumerateFiles(dir_path, "*.*", SearchOption.TopDirectoryOnly).Where(p => p.EndsWith(".jpeg") || p.EndsWith(".jpg") || p.EndsWith(".png"));

            //var s = new System.Threading.Tasks.Schedulers.LimitedConcurrencyLevelTaskScheduler(4);

            try
            {
                foreach (var item in images) //цикл параллельной обработки
                {
                    var uiContext = SynchronizationContext.Current;

                    Task.Factory.StartNew(() =>
                    {
                        return UploadAndDetectFaces(item, token);
                    }, token).ContinueWith(t =>
                    {
                        //MessageBox.Show((t.Status).ToString());
                        if (t.Status == TaskStatus.RanToCompletion)
                        {
                            var res = t.Result;
                            IList<DetectedFace> faces = (IList<DetectedFace>)res.Result;
                            foreach (var face in (IList<DetectedFace>)(res.Result))
                            {
                                //Categorization и создание классов
                                //Categorization(face, item);
                                uiContext.Send(x => Categorization(face, item), null);
                            }
                        }

                    });
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Uploads the image file and calls DetectWithStreamAsync.
        private async Task<IList<DetectedFace>> UploadAndDetectFaces(string imageFilePath, CancellationToken token)
        {

            // The list of Face attributes to return.
            IList<FaceAttributeType> faceAttributes =
                new FaceAttributeType[]
                {
            FaceAttributeType.Gender, FaceAttributeType.Age
                };

            // Call the Face API.
            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    IList<DetectedFace> faceList =
                        await faceClient.Face.DetectWithStreamAsync(
                            imageFileStream, true, false, faceAttributes, token);
                    return faceList;
                }
            }
            // Catch and display Face API errors.
            catch (APIErrorException f)
            {
                MessageBox.Show(f.Message);
                return new List<DetectedFace>();
            }
            // Catch and display all other errors.
            catch (Exception)
            {
                //MessageBox.Show(e.Message, "Error");
                return new List<DetectedFace>();
            }
        }

        static void Categorization(DetectedFace face, string imageFilePath)
        {
            lock (categories)
            {
                double age = Double.Parse(face.FaceAttributes.Age.ToString());

                if (face.FaceAttributes.Gender == Gender.Male) //Male
                {
                    if (0 <= age && age <= 19)
                        AddFace(face, CategoryEnum.Mx00_19, imageFilePath);
                    else if (20 <= age && age <= 39)
                        AddFace(face, CategoryEnum.Mx20_39, imageFilePath);
                    else if (40 <= age && age <= 59)
                        AddFace(face, CategoryEnum.Mx40_59, imageFilePath);
                    else if (60 <= age && age <= 79)
                        AddFace(face, CategoryEnum.Mx60_79, imageFilePath);
                    else if (80 <= age)
                        AddFace(face, CategoryEnum.Mx80_, imageFilePath);
                }
                else if (face.FaceAttributes.Gender == Gender.Female) //Female
                {
                    if (0 <= age && age <= 19)
                        AddFace(face, CategoryEnum.Fx00_19, imageFilePath);
                    else if (20 <= age && age <= 39)
                        AddFace(face, CategoryEnum.Fx20_39, imageFilePath);
                    else if (40 <= age && age <= 59)
                        AddFace(face, CategoryEnum.Fx40_59, imageFilePath);
                    else if (60 <= age && age <= 79)
                        AddFace(face, CategoryEnum.Fx60_79, imageFilePath);
                    else if (80 <= age)
                        AddFace(face, CategoryEnum.Fx80_, imageFilePath);
                }
                //return Category.not_determined;

            }
        }

        public static void AddFace(DetectedFace face, CategoryEnum cat, string path)
        {
            var cropped = new CroppedBitmap(new BitmapImage(new Uri(path)), new Int32Rect(
                face.FaceRectangle.Left, face.FaceRectangle.Top,
                face.FaceRectangle.Width, face.FaceRectangle.Height));
            BitmapSource source = cropped;
            BitmapImage image = ImageExtensions.BitmapSourceToBitmapImage(source);

            categories.AddCategoryFace(new Face(
                (double)face.FaceAttributes.Age, face.FaceAttributes.Gender.ToString(),
                ImageExtensions.BufferFromImage(image),
                cat));
        }

        private void InterruptCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (cts == null)
                e.CanExecute = false;
            else
            {
                if (!cts.IsCancellationRequested /*&& executing*/)
                {
                    e.CanExecute = true;
                }
                else
                    e.CanExecute = false;
            }
        }

        private void InterruptCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            cts.Cancel();
            cts.Dispose();
            cts = new CancellationTokenSource();
            token = cts.Token;
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            InitializeData();
        }

        private async void Clear_Click(object sender, RoutedEventArgs e)
        {
            categories.Clear();
            //очистка базы данных
            try
            {
                var r = await client.DeleteAsync("api/faces"); //PostAsJsonAsync("api /faces", newface);
                r.EnsureSuccessStatusCode();
            }
            catch (System.Net.Http.HttpRequestException)
            {
                MessageBox.Show("Не удается установить соединение сервером", "Ошибка очистки");
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var t = Task.Run(() => SaveToDb());
            t.Wait();
            var res = t.Result;
            if (!(bool)res)
            {
                string msg = "Отсутствует соединение с сервером.\nЗакрыть без сохранения?";
                MessageBoxResult result =
                  MessageBox.Show(
                    msg,
                    "Ошибка сохраниния",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    // If user doesn't want to close, cancel closure
                    e.Cancel = true;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var t = Task.Run(() => SaveToDb());
            //t.Wait();
            var res = t.Result;
            if (!(bool)res)
            {
                MessageBox.Show("Не удается установить соединение сервером", "Ошибка сохранения");
            }
        }
        private async Task<bool> SaveToDb()
        {
            bool result = true;
            try
            {
                //if (categories.Count > 0)
                var req = await client.DeleteAsync("api/faces"); //PostAsJsonAsync("api /faces", newface);
                req.EnsureSuccessStatusCode();
                foreach (var cat in categories)
                {
                    foreach (var face in cat.Faces)
                    {
                        var newface = new Shared.Face()
                        {
                            FaceId = face.FaceId,
                            Age = face.Age,
                            Gender = face.Gender,
                            Cat = face.Cat,
                            Bitmap = face.Bitmap,
                        };
                        var r = await client.PostAsJsonAsync("api/faces", newface);
                        r.EnsureSuccessStatusCode();
                    }
                    result = true;
                }
            }
            catch (System.Net.Http.HttpRequestException)
            {
                result = false;
                //MessageBox.Show("Не удается установить соединение сервером");
            }
            return result;
        }
    }
}
