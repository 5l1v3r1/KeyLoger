using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;
using System.IO;

namespace KeyLogger 
{
    public class KeyboardHook : IDisposable
    {
        /*----------Импортирование функций WinAPI-------------*/
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId([In] IntPtr hWnd,
            [Out, Optional] IntPtr lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern ushort GetKeyboardLayout([In] int idThread);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelKeyboardProc _proc = null;
        private IntPtr _hookID = IntPtr.Zero;
        private string fileName = null;
        private Hashtable hashTable = new Hashtable(1000);

        public KeyboardHook(string fileName)
        {
            _proc = HookCallback;
            _hookID = SetHook(_proc);
            this.fileName = fileName;
            hashTable.Clear();
            File.Delete(fileName);
        }

        //Раскладка клавиатуры активного окна
        public ushort KeyboardLayout()
        {
            IntPtr activeWindow = GetForegroundWindow();
            int activeProcess = GetWindowThreadProcessId(activeWindow, IntPtr.Zero);
            return GetKeyboardLayout(activeProcess);
        }

        //Установка хука считывание клавиш
        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                        GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        //Запись клавиш со списка в файл
        public void FlushKeys()
        {
            FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            IEnumerator e = hashTable.Keys.GetEnumerator();
            while (e.MoveNext())
            {
                sw.WriteLine(e.Current.ToString() + " = " + hashTable[e.Current].ToString());
            }
            sw.Close();
            fs.Close();
        }

        //Обработка нажатия на клавишу
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                string key = SymbolConverter.GetSymbol(
                    ((Keys)vkCode).ToString(), 
                    (Language)KeyboardLayout()
                );
                if (hashTable.Contains(key))
                {
                    hashTable[key] = ((int)hashTable[key]) + 1;
                }
                else hashTable.Add(key, 1);
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        //Освобождение ресурсов
        public void Dispose()
        {
            UnhookWindowsHookEx(_hookID);
        }
    }
}
