using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeGalleryMaker
{
    public class DownloadNameAndAddress
    {
        public string address;
        public string filename;

        public DownloadNameAndAddress(string address, string filename)
        {
            this.address = address;
            this.filename = filename;
        }
    }

    public class DownloadObject
    {
        public string websiteName;
        public string idStringNumber;
        public string idStringNumberRounded;
        public List<DownloadNameAndAddress> downloadList;
        public bool flagDLImage;

        public DownloadObject(string inputAddress)
        {
            websiteName = "";
            flagDLImage = true;

            downloadList = new List<DownloadNameAndAddress>();

            int position = 0, temp = 1, dummy = 0;

            try
            {
                for (int j = 0; j < inputAddress.Length - 2; j++)
                {
                    if (String.Equals(inputAddress.Substring(j, 2), "RJ") && char.IsDigit(inputAddress[j + 2]))
                    {
                        position = j + 2;
                        websiteName = "DLsite Japanese";
                        break;
                    }
                    else if (String.Equals(inputAddress.Substring(j, 2), "RE") && char.IsDigit(inputAddress[j + 2]))
                    {
                        position = j + 2;
                        websiteName = "DLsite English";
                        break;
                    }
                    else if (String.Equals(inputAddress.Substring(j, 2), "VJ") && char.IsDigit(inputAddress[j + 2]))
                    {
                        position = j + 2;
                        websiteName = "DLsite Professional";
                        break;
                    }
                    else if (String.Equals(inputAddress.Substring(j, 3), "id=") && char.IsDigit(inputAddress[j + 3]))
                    {
                        position = j + 3;
                        websiteName = "Getchu";
                        break;
                    }
                }

                while (true)
                {
                    try
                    {
                        idStringNumber = inputAddress.Substring(position, temp);
                        if (!int.TryParse(idStringNumber, out dummy))
                        {
                            temp--;
                            break;
                        }
                        temp++;
                    }
                    catch
                    {
                        temp--;
                        break;
                    }
                }

                idStringNumber = inputAddress.Substring(position, temp);
                idStringNumberRounded = Convert.ToString(((Convert.ToInt32(idStringNumber) / 1000) + 1) * 1000);

                if (Int32.Parse(idStringNumberRounded) < 10000) 
                {
                    idStringNumberRounded = "00" + idStringNumberRounded;
                }
                else if (Int32.Parse(idStringNumberRounded) < 100000)
                {
                    idStringNumberRounded = "0" + idStringNumberRounded;
                }
            }
            catch
            {

            }
        }

        public List<DownloadNameAndAddress> generateDownloadList()
        {
            if (String.Equals(websiteName, "DLsite Japanese"))
            {
                downloadList.Add(new DownloadNameAndAddress("http://www.dlsite.com/maniax/work/=/product_id/RJ" + idStringNumber + ".html", "RJ" + idStringNumber + ".html"));
                if (flagDLImage)
                {
                    downloadList.Add(new DownloadNameAndAddress("http://img.dlsite.jp/modpub/images2/work/doujin/RJ" + idStringNumberRounded + "/RJ" + idStringNumber + "_img_main.jpg","RJ" + idStringNumber + "_img_main.jpg"));
                    downloadList.Add(new DownloadNameAndAddress("http://img.dlsite.jp/modpub/images2/work/doujin/RE" + idStringNumberRounded + "/RE" + idStringNumber + "_img_main.jpg", "RE" + idStringNumber + "_img_main.jpg"));
                    for (int i = 1; i < 9; i++)
                    {
                        downloadList.Add(new DownloadNameAndAddress("http://img.dlsite.jp/modpub/images2/work/doujin/RJ" + idStringNumberRounded + "/RJ" + idStringNumber + "_img_smp" + i + ".jpg","RJ" + idStringNumber + "_img_smp" + i + ".jpg"));
                        downloadList.Add(new DownloadNameAndAddress("http://img.dlsite.jp/modpub/images2/work/doujin/RE" + idStringNumberRounded + "/RE" + idStringNumber + "_img_smp" + i + ".jpg","RE" + idStringNumber + "_img_smp" + i + ".jpg"));
                    }
                }
            }
            else if (String.Equals(websiteName, "DLsite English"))
            {
                downloadList.Add(new DownloadNameAndAddress("http://www.dlsite.com/maniax/work/=/product_id/RE" + idStringNumber + ".html", "RE" + idStringNumber + ".html"));
                if (flagDLImage)
                {
                    downloadList.Add(new DownloadNameAndAddress("http://img.dlsite.jp/modpub/images2/work/doujin/RJ" + idStringNumberRounded + "/RJ" + idStringNumber + "_img_main.jpg","RJ" + idStringNumber + "_img_main.jpg"));
                    downloadList.Add(new DownloadNameAndAddress("http://img.dlsite.jp/modpub/images2/work/doujin/RE" + idStringNumberRounded + "/RE" + idStringNumber + "_img_main.jpg", "RE" + idStringNumber + "_img_main.jpg"));
                    for (int i = 1; i < 9; i++)
                    {
                        downloadList.Add(new DownloadNameAndAddress("http://img.dlsite.jp/modpub/images2/work/doujin/RJ" + idStringNumberRounded + "/RJ" + idStringNumber + "_img_smp" + i + ".jpg","RJ" + idStringNumber + "_img_smp" + i + ".jpg"));
                        downloadList.Add(new DownloadNameAndAddress("http://img.dlsite.jp/modpub/images2/work/doujin/RE" + idStringNumberRounded + "/RE" + idStringNumber + "_img_smp" + i + ".jpg","RE" + idStringNumber + "_img_smp" + i + ".jpg"));
                    }
                }
            }
            else if (String.Equals(websiteName, "DLsite Professional"))
            {
                downloadList.Add(new DownloadNameAndAddress("http://www.dlsite.com/pro/work/=/product_id/VJ" + idStringNumber + ".html","VJ" + idStringNumber + ".html"));
                if (flagDLImage)
                {
                    downloadList.Add(new DownloadNameAndAddress("http://img.dlsite.jp/modpub/images2/work/professional/VJ" + idStringNumberRounded + "/VJ" + idStringNumber + "_img_main.jpg","VJ" + idStringNumber + "_img_main.jpg"));
                    for (int i = 1; i < 9; i++)
                    {
                        downloadList.Add(new DownloadNameAndAddress("http://img.dlsite.jp/modpub/images2/work/professional/VJ" + idStringNumberRounded + "/VJ" + idStringNumber + "_img_smpa" + i + ".jpg","VJ" + idStringNumber + "_img_smp" + i + ".jpg"));
                    }
                }
            }
            else if (String.Equals(websiteName, "Getchu"))
            {
                downloadList.Add(new DownloadNameAndAddress("http://www.getchu.com/brandnew/" + idStringNumber + "/c" + idStringNumber + "package.jpg","c" + idStringNumber + "package.jpg"));
                for (int n = 1; n <= 16; n++)
                {
                    downloadList.Add(new DownloadNameAndAddress("http://www.getchu.com/brandnew/" + idStringNumber + "/c" + idStringNumber + "sample" + n + ".jpg","c" + idStringNumber + "sample" + n + ".jpg"));
                }

                for (int n = 1; n <= 8; n++)
                {
                    downloadList.Add(new DownloadNameAndAddress("http://www.getchu.com/brandnew/" + idStringNumber + "/c" + idStringNumber + "table" + n + ".jpg","c" + idStringNumber + "table" + n + ".jpg"));
                }
            }
            return downloadList;
        }
    }
}
