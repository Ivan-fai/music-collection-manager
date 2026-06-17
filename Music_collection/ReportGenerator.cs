using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Words.NET;

namespace Music_collection
{
    internal class ReportGenerator : IReportGenerator
    {
        public void AlbumsGenerateReport(List<TrackList> albumsToExport)
        {
            if (albumsToExport == null || albumsToExport.Count == 0)
            {
                throw new Exception("Список відфільтрованих альбомів на екпорт пустий, неможливо створити звіт\n" +
                    "Будь ласка, оберіть інші критерії фільтрації");
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Word Document (*.docx)|*.docx";
                saveFileDialog.FileName = "Звіт_з_музичними_альбомами.docx";
                //якщо обрано зберегти у вікні
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        //створюємо пустий документ ворд по обраному шляху
                        using (DocX document = DocX.Create(saveFileDialog.FileName))
                        {
                            var titleFormat = new Xceed.Document.NET.Formatting();
                            titleFormat.FontFamily = new Xceed.Document.NET.Font("Times New Roman");
                            titleFormat.Size = 18;
                            titleFormat.Bold = true;
                            //вставляємо заголовок
                            document.InsertParagraph("Звіт про музичні альбоми\n\n", false, titleFormat)
                                    .Alignment = Xceed.Document.NET.Alignment.center;

                            //створення таблиці
                            int rowsCount = albumsToExport.Count + 1;//+1 для шапки
                            int columnsCount = 3;

                            var table = document.AddTable(rowsCount, columnsCount);
                            table.Alignment = Xceed.Document.NET.Alignment.center;
                            table.Design = Xceed.Document.NET.TableDesign.TableGrid;

                            //шапка таблиці
                            table.Rows[0].Cells[0].Paragraphs[0].Append("Назва альбому").Bold().Font("Times New Roman").FontSize(14);
                            table.Rows[0].Cells[1].Paragraphs[0].Append("Виконавець").Bold().Font("Times New Roman").FontSize(14);
                            table.Rows[0].Cells[2].Paragraphs[0].Append("Рік виходу").Bold().Font("Times New Roman").FontSize(14);

                            //заповнюємо дані таблиці із списку
                            for (int i = 0; i < albumsToExport.Count; i++)
                            {
                                table.Rows[i + 1].Cells[0].Paragraphs[0].Append(albumsToExport[i].AlbumTitle).Font("Times New Roman")
                                    .FontSize(14);
                                table.Rows[i + 1].Cells[1].Paragraphs[0].Append(albumsToExport[i].Artist).Font("Times New Roman")
                                    .FontSize(14);
                                table.Rows[i + 1].Cells[2].Paragraphs[0].Append(albumsToExport[i].ReleaseYear.ToString())
                                    .Font("Times New Roman").FontSize(14);
                            }

                            document.InsertTable(table);
                            document.Save();
                        }
                        MessageBox.Show("Дані успішно експортовано у Word");
                    }
                    catch (Exception ex)
                    { 
                        throw new Exception($"Помилка при збереженні файлу: {ex.Message}");
                    }
                }
            }
        }
        public void TracksGenerateReport(List<MusicTrack> tracksToExport)
        {
            if (tracksToExport == null || tracksToExport.Count == 0)
            {
                throw new Exception("Список відфільтрованих альбомів на екпорт пустий, неможливо створити звіт\n" +
                    "Будь ласка, оберіть інші критерії фільтрації");
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Word Document (*.docx)|*.docx";
                saveFileDialog.FileName = "Звіт_за_треками.docx";

                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        using (DocX document = DocX.Create(saveFileDialog.FileName))
                        {
                            var titleFormat = new Xceed.Document.NET.Formatting();
                            titleFormat.FontFamily = new Xceed.Document.NET.Font("Times New Roman");
                            titleFormat.Size = 18;
                            titleFormat.Bold = true;

                            document.InsertParagraph("Звіт про музичні композиції\n\n", false, titleFormat)
                                    .Alignment = Xceed.Document.NET.Alignment.center;

                            int rowsCount = tracksToExport.Count + 1;
                            int columnsCount = 3;

                            var table = document.AddTable(rowsCount, columnsCount);
                            table.Alignment = Xceed.Document.NET.Alignment.center;
                            table.Design = Xceed.Document.NET.TableDesign.TableGrid;

                            //шапка
                            table.Rows[0].Cells[0].Paragraphs[0].Append("Назва треку").Bold().Font("Times New Roman").FontSize(14);
                            table.Rows[0].Cells[1].Paragraphs[0].Append("Автор").Bold().Font("Times New Roman").FontSize(14);
                            table.Rows[0].Cells[2].Paragraphs[0].Append("Альбом").Bold().Font("Times New Roman").FontSize(14);

                            //заповнення даних
                            for (int i = 0; i < tracksToExport.Count; i++)
                            {
                                table.Rows[i + 1].Cells[0].Paragraphs[0].Append(tracksToExport[i].Title)
                                    .Font("Times New Roman").FontSize(14);
                                table.Rows[i + 1].Cells[1].Paragraphs[0].Append(tracksToExport[i].Author)
                                    .Font("Times New Roman").FontSize(14);
                                table.Rows[i + 1].Cells[2].Paragraphs[0].Append(tracksToExport[i].Album)
                                    .Font("Times New Roman").FontSize(14);
                            }

                            document.InsertTable(table);
                            document.Save();
                        }

                        MessageBox.Show("Треки успішно експортовано у Word");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Помилка при збереженні файлу: {ex.Message}");
                    }
                }
            }
        }
    }
}
