﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Soulseek_Sorter
{
    public class DataStorage
    {
        public void saveCompletedFolder(string filePath)
        {
            File.WriteAllText("CompleteFolderSave.txt", filePath);
        }

        public void saveDestinationFolder(string filePath)
        {
            File.WriteAllText("DestinationFolderSave.txt", filePath);
        }

        public string loadDestinationFolderAsString()
        {
            string path;
            if (File.Exists("DestinationFolderSave.txt"))
            {
                path = File.ReadAllText("DestinationFolderSave.txt");

            }
            else
            {
                path = "Empty";
            }
            return path;
        }
        public string loadCompletedFolderAsString()
        {
            string path;
            if (File.Exists("CompleteFolderSave.txt"))
            {
                path = File.ReadAllText("CompleteFolderSave.txt");
                
            }
            else
            {
                path = "Empty";
            }
            return path;
        }
    }
}
