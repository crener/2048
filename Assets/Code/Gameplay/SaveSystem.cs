using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Code.Gameplay;
using UnityEngine;

namespace Assets.Code.Gameplay
{
    static class SaveSystem
    {
        public static void SaveBoard(BoardState state)
        {
            if (state.Score == 0) return;
            if (state.BoardHeight == 0) return;
            if (state.BoardWidth == 0) return;

            string data = JsonUtility.ToJson(state);
            string path = Application.persistentDataPath + Path.AltDirectorySeparatorChar +
                          state.BoardWidth + state.BoardHeight;

            try
            {
                using (StreamWriter writer = new StreamWriter(path + ".save", false))
                {
                    writer.Write(data);
                }

                SaveScore(state.Score, state.BoardWidth, state.BoardHeight);
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR
                Debug.Log("SaveException: " + ex);
#endif 
            }
        }

        public static void SaveScore(int newScore, int width, int height)
        {
            string path = Application.persistentDataPath + Path.AltDirectorySeparatorChar +
                          width + height + "h.save";
            try
            {
                int prevScore = 0;
                if (File.Exists(path))
                {
                    using (StreamReader reader = new StreamReader(path, false))
                        prevScore = int.Parse(reader.ReadToEnd());
                }

                if (newScore > prevScore)
                {
                    using (StreamWriter writer = new StreamWriter(path, false))
                        writer.Write(newScore);
                }
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR
                Debug.Log("SaveException: " + ex);
#endif
            }
        }

        public static int LoadScore(int width, int height)
        {
            if (width == 0 || height == 0) return 0;
            string path = Application.persistentDataPath + Path.AltDirectorySeparatorChar +
                          width + height + "h.save";

            if (File.Exists(path))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(path, false))
                        return int.Parse(reader.ReadToEnd());
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR
                    Debug.Log("SaveException: " + ex);
#endif
                }
            }

            return 0;
        }
        public static BoardState LoadBoard(int width, int height)
        {
            string path = Application.persistentDataPath + Path.AltDirectorySeparatorChar +
                          width + height + ".save", data = "";
            if (File.Exists(path))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(path, false))
                        return JsonUtility.FromJson<BoardState>(reader.ReadToEnd());
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR
                    Debug.Log("SaveException: " + ex);
#endif
                }
            }

            return null;
        }

        public static bool HasBoard(int width, int height)
        {
            string path = Application.persistentDataPath + Path.AltDirectorySeparatorChar +
                          width + height + ".save";

            return File.Exists(path);
        }

        public static void RemoveBoard(int width, int height)
        {
            string path = Application.persistentDataPath + Path.AltDirectorySeparatorChar +
                          width + height + ".save";

            if (File.Exists(path)) File.Delete(path);
        }
    }
}
