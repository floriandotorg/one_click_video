using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using one_click_video.Resources;

namespace one_click_video
{
    static class Helper
    {
        static readonly Random rnd = new Random();

        public static IList<T> Shuffle<T>(this IList<T> input)
        {
            for (var top = input.Count - 1; top > 1; --top)
            {
                var swap = rnd.Next(0, top);
                T tmp = input[top];
                input[top] = input[swap];
                input[swap] = tmp;
            }

            return input;
        }

        public static IList<T> ShuffleCopy<T>(this IEnumerable<T> input)
        { return new List<T>(input).Shuffle(); }
    }


    class TileUtil
    {
        private static T min<T>(T a, T b) where T : System.IComparable<T>
        {
            return (a.CompareTo(b) < 0) ? a : b;
        }

        public static void UpdateTile()
        {
            ShellTile tile = ShellTile.ActiveTiles.First();

            CycleTileData tileData = new CycleTileData();

            tileData.Title = AppResources.Videos;
            tileData.SmallBackgroundImage = new Uri("Assets/Tiles/FlipCycleTileSmall.png", UriKind.Relative);


            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var fileNames = new List<string>();

                foreach (var file in storage.GetFileNames())
                {
                    if (file.EndsWith(".jpg"))
                    {
                        fileNames.Add(file);
                    }
                }

                fileNames.Shuffle();

                var fileList = new List<Uri>();
                int count = 0;
                foreach (var file in fileNames.GetRange(0,min(fileNames.Count,9)))
                {
                    ++count;
                    storage.CopyFile(file, "Shared/ShellContent/img_" + count + ".jpg", true);
                    fileList.Add(new Uri("isostore:/shared/ShellContent/img_" + count + ".jpg", UriKind.Absolute));
                }

                if (fileList.Count == 0)
                {
                    fileList.Add(new Uri("Assets/Tiles/FlipCycleTileMedium.png", UriKind.Relative));
                }

                tileData.CycleImages = fileList;
            }

            tile.Update(tileData);
        }
    }
}
