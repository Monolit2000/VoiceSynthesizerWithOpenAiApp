//using NAudio.Wave;
//using System;
//using System.IO;
//using System.Media;

//public class AudioTrimmer
//{
//    public static void Trim(string inputPath, string outputPath, int startTime, int endTime)
//    {
//        // Чтение аудио файла из входного пути
//        using (Stream inputStream = File.OpenRead(inputPath))
//        {
//            // Создание экземпляра AudioFileReader
//            using (AudioFileReader audioFileReader = new AudioFileReader(inputStream))
//            {
//                // Получение длины аудио файла
//                var length = audioFileReader.Length;

//                // Проверка, что начальное время меньше конечного времени
//                if (startTime > endTime)
//                {
//                    throw new ArgumentException("Начальное время должно быть меньше конечного времени.");
//                }

//                // Создание экземпляра AudioFileWriter
//                using (AudioFileWriter audioFileWriter = new AudioFileWriter(outputPath))
//                {
//                    // Запись аудио данных в выходной файл
//                    audioFileWriter.Write(audioFileReader.Read(startTime, length - startTime));
//                }
//            }
//        }
//    }
//}
