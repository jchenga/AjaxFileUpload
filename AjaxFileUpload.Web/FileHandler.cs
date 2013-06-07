using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace AjaxFileUpload.Web
{
    public class FileHandler
    {
        private const int ChunkLength = 64 * 1024;
        private String _basePath;
        private Guid _id;
        private String _fullPath;

        public FileHandler(String basePath, Guid id) 
        {
            _basePath = basePath;
            _id = id;
            _fullPath = BuildFullPath(basePath, id);
        }

        public void SaveFile(long contentSize, Stream stream, Action<int> action) {
            String realFileName = String.Empty;

            using (FileStream fs = new FileStream(_fullPath, FileMode.OpenOrCreate))
            {
                int maxIterations = (int)(contentSize / (ChunkLength));
                int dataCaptured = 0;
                String fileHeader = String.Empty;
                
                byte[] buffer = new byte[ChunkLength];
                for (int i = 0; i <= maxIterations; i++)
                {
                    if (i < maxIterations)
                    {
                        stream.Read(buffer, 0, ChunkLength);
                        dataCaptured += ChunkLength;
                    }
                    else
                    {
                        buffer = new byte[(int)stream.Length - dataCaptured];
                        stream.Read(buffer, 0, buffer.Length);
                    }

                    int positionToRemove = 0;
                    int bytesToGet = buffer.Length;
                    
                    if (i == 0)
                    {
                        int numberOfFounds = 0;
                        bool canBe = false;
                        for (int j = 0; j < buffer.Length; j++)
                        {
                            if (buffer[j] == 13)
                            {
                                canBe = true;
                            }
                            else
                            {
                                if (canBe && buffer[j] == 10)
                                {
                                    numberOfFounds++;
                                    if (numberOfFounds == 4)
                                    {
                                        positionToRemove = j + 1;
                                        byte[] headerBytes = new byte[positionToRemove];
                                        Array.Copy(buffer, headerBytes, positionToRemove);
                                        fileHeader = UnicodeEncoding.UTF8.GetString(headerBytes);
                                        bytesToGet = buffer.Length - positionToRemove;
                                        break;
                                    }
                                }
                                else
                                {
                                    canBe = false;
                                }
                            }
                        }

                    }


                    // why do we need the if statement here? what does it do?
                    if ((int)stream.Length - buffer.Length - dataCaptured <= 49)
                    {
                        bytesToGet = bytesToGet - 48;
                    }

                    fs.Write(buffer, positionToRemove, bytesToGet);
                    double progress = ((i + 1) * ((double) contentSize / maxIterations) / contentSize) * 100;
                    action((int) Math.Floor(progress));
                }
                int filenameStartIndex = fileHeader.IndexOf("filename=\"") + "filename=\"".Length;
                int fileNameEndIndex = fileHeader.Substring(filenameStartIndex).IndexOf("\"");
                realFileName = Path.GetFileName(fileHeader.Substring(filenameStartIndex, fileNameEndIndex));
            }

            File.Move(_fullPath, _fullPath.Replace(_id.ToString(), realFileName));
        }

        private String BuildFullPath(string basePath, Guid id)
        {
            return Path.Combine(basePath, id.ToString());
        }
    }
}