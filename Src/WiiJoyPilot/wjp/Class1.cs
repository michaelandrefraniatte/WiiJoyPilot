using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Numerics;
using Microsoft.Win32.SafeHandles;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using static System.Windows.Forms.AxHost;
using System.Collections;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net;

namespace wjp
{
    public class Class1
    {
        [DllImport("hid.dll")]
        private static extern void HidD_GetHidGuid(out Guid gHid);
        [DllImport("hid.dll")]
        private extern static bool HidD_SetOutputReport(IntPtr HidDeviceObject, byte[] lpReportBuffer, uint ReportBufferLength);
        [DllImport("setupapi.dll")]
        private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, string Enumerator, IntPtr hwndParent, UInt32 Flags);
        [DllImport("setupapi.dll")]
        private static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInvo, ref Guid interfaceClassGuid, Int32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);
        [DllImport("setupapi.dll")]
        private static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, UInt32 deviceInterfaceDetailDataSize, out UInt32 requiredSize, IntPtr deviceInfoData);
        [DllImport("setupapi.dll")]
        private static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, UInt32 deviceInterfaceDetailDataSize, out UInt32 requiredSize, IntPtr deviceInfoData);
        [DllImport("Kernel32.dll")]
        private static extern SafeFileHandle CreateFile(string fileName, [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess, [MarshalAs(UnmanagedType.U4)] FileShare fileShare, IntPtr securityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition, [MarshalAs(UnmanagedType.U4)] uint flags, IntPtr template);
        [DllImport("Kernel32.dll")]
        private static extern IntPtr CreateFile(string fileName, System.IO.FileAccess fileAccess, System.IO.FileShare fileShare, IntPtr securityAttributes, System.IO.FileMode creationDisposition, EFileAttributes flags, IntPtr template);
        [DllImport("lhidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Lhid_read_timeout")]
        private static unsafe extern int Lhid_read_timeout(SafeFileHandle dev, byte[] data, UIntPtr length);
        [DllImport("lhidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Lhid_write")]
        private static unsafe extern int Lhid_write(SafeFileHandle device, byte[] data, UIntPtr length);
        [DllImport("lhidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Lhid_open_path")]
        private static unsafe extern SafeFileHandle Lhid_open_path(IntPtr handle);
        [DllImport("lhidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Lhid_close")]
        private static unsafe extern void Lhid_close(SafeFileHandle device);
        private static bool back, start, A, B, X, Y, up, left, down, right, leftstick, rightstick, leftbumper, rightbumper, lefttrigger, righttrigger;
        private static double leftstickx, leftsticky, rightstickx, rightsticky;
        public const double REGISTER_IR = 0x04b00030, REGISTER_EXTENSION_INIT_1 = 0x04a400f0, REGISTER_EXTENSION_INIT_2 = 0x04a400fb, REGISTER_EXTENSION_TYPE = 0x04a400fa, REGISTER_EXTENSION_CALIBRATION = 0x04a40020, REGISTER_MOTIONPLUS_INIT = 0x04a600fe;
        public static double irx0, iry0, irx1, iry1, irx, iry, irxc, iryc, WiimoteIRSensors0X, WiimoteIRSensors0Y, WiimoteIRSensors1X, WiimoteIRSensors1Y, WiimoteRawValuesX, WiimoteRawValuesY, WiimoteRawValuesZ, calibrationinit, tickviewxinit, stickviewyinit, WiimoteNunchuckStateRawValuesX, WiimoteNunchuckStateRawValuesY, WiimoteNunchuckStateRawValuesZ, WiimoteNunchuckStateRawJoystickX, WiimoteNunchuckStateRawJoystickY, mousex, mousey, mousexp, mouseyp, WiimoteIRSensors0Xcam, WiimoteIRSensors0Ycam, WiimoteIRSensors1Xcam, WiimoteIRSensors1Ycam, WiimoteIRSensorsXcam, WiimoteIRSensorsYcam, WiimoteIR0notfound = 0, irx2, iry2, irx3, iry3;
        public static bool WiimoteIR1foundcam, WiimoteIR0foundcam, WiimoteIR1found, WiimoteIR0found, WiimoteIRswitch, WiimoteButtonStateA, WiimoteButtonStateB, WiimoteButtonStateMinus, WiimoteButtonStateHome, WiimoteButtonACC, WiimoteButtonStatePlus, WiimoteButtonStateOne, WiimoteButtonStateTwo, WiimoteButtonStateUp, WiimoteButtonStateDown, WiimoteButtonStateLeft, WiimoteButtonStateRight, WiimoteNunchuckStateC, WiimoteNunchuckStateZ, ISWIIMOTE, running, enabling, scriptenabling;
        public static byte[] buff = new byte[] { 0x55 }, mBuff = new byte[22], aBuffer = new byte[22];
        public const byte Type = 0x12, IR = 0x13, WriteMemory = 0x16, ReadMemory = 0x16, IRExtensionAccel = 0x37;
        public static FileStream mStream;
        public static SafeFileHandle handle = null;
        private static Type program;
        private static object obj;
        private static string code = @"
                using System;
                using System.Runtime.InteropServices;
                namespace StringToCode
                {
                    public class FooClass 
                    { 
                        bool back, start, A, B, X, Y, up, left, down, right, leftstick, rightstick, leftbumper, rightbumper, lefttrigger, righttrigger;
                        public bool[] getstate = new bool[36];
                        public static int[] wd = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
                        public static int[] wu = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
                        public static void valchanged(int n, bool val)
                        {
                            if (val)
                            {
                                if (wd[n] <= 1)
                                {
                                    wd[n] = wd[n] + 1;
                                }
                                wu[n] = 0;
                            }
                            else
                            {
                                if (wu[n] <= 1)
                                {
                                    wu[n] = wu[n] + 1;
                                }
                                wd[n] = 0;
                            }
                        }
                        public object[] Main(bool adsswitch, bool JoyconLeftButtonDPAD_DOWN, bool JoyconLeftButtonDPAD_LEFT, bool JoyconLeftButtonDPAD_RIGHT, bool JoyconLeftButtonDPAD_UP, bool JoyconLeftButtonMINUS, bool JoyconLeftButtonACC, bool JoyconLeftButtonSHOULDER_1, bool JoyconLeftButtonSHOULDER_2, bool JoyconLeftButtonCAPTURE, bool WiimoteButtonStateOne, bool WiimoteButtonStateTwo, bool WiimoteButtonStateDown, bool WiimoteButtonStateLeft, bool WiimoteButtonStateRight, bool WiimoteButtonStateUp, bool WiimoteButtonStateHome, bool WiimoteButtonACC, bool WiimoteButtonStateA, bool WiimoteButtonStateB, bool WiimoteButtonStatePlus, bool WiimoteButtonStateMinus)
                        {
                            funct_driver
                            return new object[] { back, start, A, B, X, Y, up, left, down, right, leftstick, rightstick, leftbumper, rightbumper, lefttrigger, righttrigger };
                        }
                    }
                }";
        private static string finalcode = "", funct_driver = "";
        private static string cbback1select; private static string cbback2select; private static string cbstart1select; private static string cbstart2select; private static string cba1select; private static string cba2select; private static string cbb1select; private static string cbb2select; private static string cbx1select; private static string cbx2select; private static string cby1select; private static string cby2select; private static string cbup1select; private static string cbup2select; private static string cbleft1select; private static string cbleft2select; private static string cbdown1select; private static string cbdown2select; private static string cbright1select; private static string cbright2select; private static string cbrightstick1select; private static string cbrightstick2select; private static string cbleftstick1select; private static string cbleftstick2select; private static string cbrightbumper1select; private static string cbrightbumper2select; private static string cbleftbumper1select; private static string cbleftbumper2select; private static string cbrighttrigger1select; private static string cbrighttrigger2select; private static string cblefttrigger1select; private static string cblefttrigger2select; private static double irmode = 1; private static double centery = 160f; private static bool adsswitch; private static bool movesmooth; private static double irxsens; private static double irysens; private static double stickxsens; private static double stickysens; private static double viewpower1x; private static double viewpower2x; private static double viewpower3x; private static double viewpower1y; private static double viewpower2y; private static double viewpower3y; private static double dzx; private static double dzy;
        public static bool[] getstate = new bool[36];
        public static int[] wd = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
        public static int[] wu = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
        public static void valchanged(int n, bool val)
        {
            if (val)
            {
                if (wd[n] <= 1)
                {
                    wd[n] = wd[n] + 1;
                }
                wu[n] = 0;
            }
            else
            {
                if (wu[n] <= 1)
                {
                    wu[n] = wu[n] + 1;
                }
                wd[n] = 0;
            }
        }
        public static void Apply(string cbback1selectapply, string cbback2selectapply, string cbstart1selectapply, string cbstart2selectapply, string cba1selectapply, string cba2selectapply, string cbb1selectapply, string cbb2selectapply, string cbx1selectapply, string cbx2selectapply, string cby1selectapply, string cby2selectapply, string cbup1selectapply, string cbup2selectapply, string cbleft1selectapply, string cbleft2selectapply, string cbdown1selectapply, string cbdown2selectapply, string cbright1selectapply, string cbright2selectapply, string cbrightstick1selectapply, string cbrightstick2selectapply, string cbleftstick1selectapply, string cbleftstick2selectapply, string cbrightbumper1selectapply, string cbrightbumper2selectapply, string cbleftbumper1selectapply, string cbleftbumper2selectapply, string cbrighttrigger1selectapply, string cbrighttrigger2selectapply, string cblefttrigger1selectapply, string cblefttrigger2selectapply, double irmodeapply, double centeryapply, bool adsswitchapply, bool movesmoothapply, double irxsensapply, double irysensapply, double stickxsensapply, double stickysensapply, double viewpower1xapply, double viewpower2xapply, double viewpower3xapply, double viewpower1yapply, double viewpower2yapply, double viewpower3yapply, double dzxapply, double dzyapply)
        {
            scriptenabling = false;
            cbback1select = cbback1selectapply;
            cbback2select = cbback2selectapply;
            cbstart1select = cbstart1selectapply;
            cbstart2select = cbstart2selectapply;
            cba1select = cba1selectapply;
            cba2select = cba2selectapply;
            cbb1select = cbb1selectapply;
            cbb2select = cbb2selectapply;
            cbx1select = cbx1selectapply;
            cbx2select = cbx2selectapply;
            cby1select = cby1selectapply;
            cby2select = cby2selectapply;
            cbup1select = cbup1selectapply;
            cbup2select = cbup2selectapply;
            cbleft1select = cbleft1selectapply;
            cbleft2select = cbleft2selectapply;
            cbdown1select = cbdown1selectapply;
            cbdown2select = cbdown2selectapply;
            cbright1select = cbright1selectapply;
            cbright2select = cbright2selectapply;
            cbrightstick1select = cbrightstick1selectapply;
            cbrightstick2select = cbrightstick2selectapply;
            cbleftstick1select = cbleftstick1selectapply;
            cbleftstick2select = cbleftstick2selectapply;
            cbrightbumper1select = cbrightbumper1selectapply;
            cbrightbumper2select = cbrightbumper2selectapply;
            cbleftbumper1select = cbleftbumper1selectapply;
            cbleftbumper2select = cbleftbumper2selectapply;
            cbrighttrigger1select = cbrighttrigger1selectapply;
            cbrighttrigger2select = cbrighttrigger2selectapply;
            cblefttrigger1select = cblefttrigger1selectapply;
            cblefttrigger2select = cblefttrigger2selectapply;
            irmode = irmodeapply;
            centery = centeryapply;
            adsswitch = adsswitchapply;
            movesmooth = movesmoothapply;
            irxsens = irxsensapply;
            irysens = irysensapply;
            stickxsens = stickxsensapply;
            stickysens = stickysensapply;
            if (viewpower1xapply == 0f & viewpower2xapply == 0f & viewpower3xapply == 0f)
            {
                viewpower1x = 0.333f;
                viewpower2x = 0.333f;
                viewpower3x = 0.333f;
            }
            else
            {
                viewpower1x = viewpower1xapply / (viewpower1xapply + viewpower2xapply + viewpower3xapply);
                viewpower2x = viewpower2xapply / (viewpower1xapply + viewpower2xapply + viewpower3xapply);
                viewpower3x = viewpower3xapply / (viewpower1xapply + viewpower2xapply + viewpower3xapply);
            }
            if (viewpower1yapply == 0f & viewpower2yapply == 0f & viewpower3yapply == 0f)
            {
                viewpower1y = 0.333f;
                viewpower2y = 0.333f;
                viewpower3y = 0.333f;
            }
            else
            {
                viewpower1y = viewpower1yapply / (viewpower1yapply + viewpower2yapply + viewpower3yapply);
                viewpower2y = viewpower2yapply / (viewpower1yapply + viewpower2yapply + viewpower3yapply);
                viewpower3y = viewpower3yapply / (viewpower1yapply + viewpower2yapply + viewpower3yapply);
            }
            dzx = dzxapply;
            dzy = dzyapply;
            funct_driver = @"
                back = back1 | back2;
                start = start1 | start2;
                A = a1 | a2;
                B = b1 | b2;
                X = x1 | x2;
                Y = y1 | y2;
                up = up1 | up2;
                left = left1 | left2;
                down = down1 | down2;
                right = right1 | right2;
                leftstick = leftstick1 | leftstick2;
                rightstick = rightstick1 | rightstick2;
                leftbumper = leftbumper1 | leftbumper2;
                rightbumper = rightbumper1 | rightbumper2;
                righttrigger = righttrigger1 | righttrigger2;
                if (!adsswitch)
                {
                    lefttrigger = lefttrigger1 | lefttrigger2;
                }
                else
                {
                    valchanged(0, lefttrigger1 | lefttrigger2);
                    if (wd[0] == 1 & !getstate[0])
                    {
                        getstate[0] = true;
                    }
                    else
                    {
                        if (wd[0] == 1 & getstate[0])
                        {
                            getstate[0] = false;
                        }
                    }
                    if (X | Y | rightbumper | leftbumper | rightstick | leftstick | back | start)
                    {
                        getstate[0] = false;
                    }
                    lefttrigger = getstate[0];
                }";
            if (cbback1select == "")
                funct_driver = funct_driver.Replace(" back1", " false");
            else
                funct_driver = funct_driver.Replace(" back1", " " + cbback1select);
            if (cbback2select == "")
                funct_driver = funct_driver.Replace(" back2", " false");
            else
                funct_driver = funct_driver.Replace(" back2", " " + cbback2select);
            if (cbstart1select == "")
                funct_driver = funct_driver.Replace(" start1", " false");
            else
                funct_driver = funct_driver.Replace(" start1", " " + cbstart1select);
            if (cbstart2select == "")
                funct_driver = funct_driver.Replace(" start2", " false");
            else
                funct_driver = funct_driver.Replace(" start2", " " + cbstart2select);
            if (cba1select == "")
                funct_driver = funct_driver.Replace(" a1", " false");
            else
                funct_driver = funct_driver.Replace(" a1", " " + cba1select);
            if (cba2select == "")
                funct_driver = funct_driver.Replace(" a2", " false");
            else
                funct_driver = funct_driver.Replace(" a2", " " + cba2select);
            if (cbb1select == "")
                funct_driver = funct_driver.Replace(" b1", " false");
            else
                funct_driver = funct_driver.Replace(" b1", " " + cbb1select);
            if (cbb2select == "")
                funct_driver = funct_driver.Replace(" b2", " false");
            else
                funct_driver = funct_driver.Replace(" b2", " " + cbb2select);
            if (cbx1select == "")
                funct_driver = funct_driver.Replace(" x1", " false");
            else
                funct_driver = funct_driver.Replace(" x1", " " + cbx1select);
            if (cbx2select == "")
                funct_driver = funct_driver.Replace(" x2", " false");
            else
                funct_driver = funct_driver.Replace(" x2", " " + cbx2select);
            if (cby1select == "")
                funct_driver = funct_driver.Replace(" y1", " false");
            else
                funct_driver = funct_driver.Replace(" y1", " " + cby1select);
            if (cby2select == "")
                funct_driver = funct_driver.Replace(" y2", " false");
            else
                funct_driver = funct_driver.Replace(" y2", " " + cby2select);
            if (cbup1select == "")
                funct_driver = funct_driver.Replace(" up1", " false");
            else
                funct_driver = funct_driver.Replace(" up1", " " + cbup1select);
            if (cbup2select == "")
                funct_driver = funct_driver.Replace(" up2", " false");
            else
                funct_driver = funct_driver.Replace(" up2", " " + cbup2select);
            if (cbleft1select == "")
                funct_driver = funct_driver.Replace(" left1", " false");
            else
                funct_driver = funct_driver.Replace(" left1", " " + cbleft1select);
            if (cbleft2select == "")
                funct_driver = funct_driver.Replace(" left2", " false");
            else
                funct_driver = funct_driver.Replace(" left2", " " + cbleft2select);
            if (cbdown1select == "")
                funct_driver = funct_driver.Replace(" down1", " false");
            else
                funct_driver = funct_driver.Replace(" down1", " " + cbdown1select);
            if (cbdown2select == "")
                funct_driver = funct_driver.Replace(" down2", " false");
            else
                funct_driver = funct_driver.Replace(" down2", " " + cbdown2select);
            if (cbright1select == "")
                funct_driver = funct_driver.Replace(" right1", " false");
            else
                funct_driver = funct_driver.Replace(" right1", " " + cbright1select);
            if (cbright2select == "")
                funct_driver = funct_driver.Replace(" right2", " false");
            else
                funct_driver = funct_driver.Replace(" right2", " " + cbright2select);
            if (cbrightstick1select == "")
                funct_driver = funct_driver.Replace(" rightstick1", " false");
            else
                funct_driver = funct_driver.Replace(" rightstick1", " " + cbrightstick1select);
            if (cbrightstick2select == "")
                funct_driver = funct_driver.Replace(" rightstick2", " false");
            else
                funct_driver = funct_driver.Replace(" rightstick2", " " + cbrightstick2select);
            if (cbleftstick1select == "")
                funct_driver = funct_driver.Replace(" leftstick1", " false");
            else
                funct_driver = funct_driver.Replace(" leftstick1", " " + cbleftstick1select);
            if (cbleftstick2select == "")
                funct_driver = funct_driver.Replace(" leftstick2", " false");
            else
                funct_driver = funct_driver.Replace(" leftstick2", " " + cbleftstick2select);
            if (cbrightbumper1select == "")
                funct_driver = funct_driver.Replace(" rightbumper1", " false");
            else
                funct_driver = funct_driver.Replace(" rightbumper1", " " + cbrightbumper1select);
            if (cbrightbumper2select == "")
                funct_driver = funct_driver.Replace(" rightbumper2", " false");
            else
                funct_driver = funct_driver.Replace(" rightbumper2", " " + cbrightbumper2select);
            if (cbleftbumper1select == "")
                funct_driver = funct_driver.Replace(" leftbumper1", " false");
            else
                funct_driver = funct_driver.Replace(" leftbumper1", " " + cbleftbumper1select);
            if (cbleftbumper2select == "")
                funct_driver = funct_driver.Replace(" leftbumper2", " false");
            else
                funct_driver = funct_driver.Replace(" leftbumper2", " " + cbleftbumper2select);
            if (cbrighttrigger1select == "")
                funct_driver = funct_driver.Replace(" righttrigger1", " false");
            else
                funct_driver = funct_driver.Replace(" righttrigger1", " " + cbrighttrigger1select);
            if (cbrighttrigger2select == "")
                funct_driver = funct_driver.Replace(" righttrigger2", " false");
            else
                funct_driver = funct_driver.Replace(" righttrigger2", " " + cbrighttrigger2select);
            if (cblefttrigger1select == "")
                funct_driver = funct_driver.Replace(" lefttrigger1", " false");
            else
                funct_driver = funct_driver.Replace(" lefttrigger1", " " + cblefttrigger1select);
            if (cblefttrigger2select == "")
                funct_driver = funct_driver.Replace(" lefttrigger2", " false");
            else
                funct_driver = funct_driver.Replace(" lefttrigger2", " " + cblefttrigger2select);
            finalcode = code.Replace("funct_driver", funct_driver);
            System.CodeDom.Compiler.CompilerParameters parameters = new System.CodeDom.Compiler.CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            parameters.ReferencedAssemblies.Add("System.Drawing.dll");
            Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.CompilerResults results = provider.CompileAssemblyFromSource(parameters, finalcode);
            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();
                foreach (System.CodeDom.Compiler.CompilerError error in results.Errors)
                {
                    sb.AppendLine(String.Format("Error ({0}) : {1}", error.ErrorNumber, error.ErrorText));
                }
                MessageBox.Show("Script Error :\n\r" + sb.ToString());
                return;
            }
            Assembly assembly = results.CompiledAssembly;
            program = assembly.GetType("StringToCode.FooClass");
            obj = Activator.CreateInstance(program);
            scriptenabling = true;
        }
        public static void Start()
        {
            running = true;
            do
                Thread.Sleep(1);
            while (!wiimotejoyconleftconnect());
            ScanScanLeft();
            Task.Run(() => taskD());
            Task.Run(() => taskDLeft());
            Thread.Sleep(1000);
            calibrationinit = -aBuffer[4] + 135f;
            stick_rawLeft[0] = report_bufLeft[6 + (ISLEFT ? 0 : 3)];
            stick_rawLeft[1] = report_bufLeft[7 + (ISLEFT ? 0 : 3)];
            stick_rawLeft[2] = report_bufLeft[8 + (ISLEFT ? 0 : 3)];
            stickCenterLeft[0] = (UInt16)(stick_rawLeft[0] | ((stick_rawLeft[1] & 0xf) << 8));
            stickCenterLeft[1] = (UInt16)((stick_rawLeft[1] >> 4) | (stick_rawLeft[2] << 4));
            acc_gcalibrationLeftX = (Int16)(report_bufLeft[13] | ((report_bufLeft[14] << 8) & 0xff00));
            acc_gcalibrationLeftY = (Int16)(report_bufLeft[15] | ((report_bufLeft[16] << 8) & 0xff00));
            acc_gcalibrationLeftZ = (Int16)(report_bufLeft[17] | ((report_bufLeft[18] << 8) & 0xff00));
            acc_gLeft.X = ((Int16)(report_bufLeft[13] | ((report_bufLeft[14] << 8) & 0xff00)) - acc_gcalibrationLeftX) * (1.0f / 4000f);
            acc_gLeft.Y = ((Int16)(report_bufLeft[15] | ((report_bufLeft[16] << 8) & 0xff00)) - acc_gcalibrationLeftY) * (1.0f / 4000f);
            acc_gLeft.Z = ((Int16)(report_bufLeft[17] | ((report_bufLeft[18] << 8) & 0xff00)) - acc_gcalibrationLeftZ) * (1.0f / 4000f);
            InitDirectAnglesLeft = acc_gLeft;
            ScpBus.LoadController();
            enabling = true;
        }
        public static void taskB()
        {
            if (enabling)
            {
                if (irmode == 1)
                {
                    WiimoteIRSensors0X = aBuffer[6] | ((aBuffer[8] >> 4) & 0x03) << 8;
                    WiimoteIRSensors0Y = aBuffer[7] | ((aBuffer[8] >> 6) & 0x03) << 8;
                    WiimoteIRSensors1X = aBuffer[9] | ((aBuffer[8] >> 0) & 0x03) << 8;
                    WiimoteIRSensors1Y = aBuffer[10] | ((aBuffer[8] >> 2) & 0x03) << 8;
                    WiimoteIR0found = WiimoteIRSensors0X > 0f & WiimoteIRSensors0X <= 1024f & WiimoteIRSensors0Y > 0f & WiimoteIRSensors0Y <= 768f;
                    WiimoteIR1found = WiimoteIRSensors1X > 0f & WiimoteIRSensors1X <= 1024f & WiimoteIRSensors1Y > 0f & WiimoteIRSensors1Y <= 768f;
                    if (WiimoteIR0found)
                    {
                        WiimoteIRSensors0Xcam = WiimoteIRSensors0X - 512f;
                        WiimoteIRSensors0Ycam = WiimoteIRSensors0Y - 384f;
                    }
                    if (WiimoteIR1found)
                    {
                        WiimoteIRSensors1Xcam = WiimoteIRSensors1X - 512f;
                        WiimoteIRSensors1Ycam = WiimoteIRSensors1Y - 384f;
                    }
                    if (WiimoteIR0found & WiimoteIR1found)
                    {
                        WiimoteIRSensorsXcam = (WiimoteIRSensors0Xcam + WiimoteIRSensors1Xcam) / 2f;
                        WiimoteIRSensorsYcam = (WiimoteIRSensors0Ycam + WiimoteIRSensors1Ycam) / 2f;
                    }
                    if (WiimoteIR0found)
                    {
                        irx0 = 2 * WiimoteIRSensors0Xcam - WiimoteIRSensorsXcam;
                        iry0 = 2 * WiimoteIRSensors0Ycam - WiimoteIRSensorsYcam;
                    }
                    if (WiimoteIR1found)
                    {
                        irx1 = 2 * WiimoteIRSensors1Xcam - WiimoteIRSensorsXcam;
                        iry1 = 2 * WiimoteIRSensors1Ycam - WiimoteIRSensorsYcam;
                    }
                    irxc = irx0 + irx1;
                    iryc = iry0 + iry1;
                }
                else
                {
                    WiimoteIR0found = (aBuffer[6] | ((aBuffer[8] >> 4) & 0x03) << 8) > 1 & (aBuffer[6] | ((aBuffer[8] >> 4) & 0x03) << 8) < 1023;
                    WiimoteIR1found = (aBuffer[9] | ((aBuffer[8] >> 0) & 0x03) << 8) > 1 & (aBuffer[9] | ((aBuffer[8] >> 0) & 0x03) << 8) < 1023;
                    if (WiimoteIR0notfound == 0 & WiimoteIR1found)
                        WiimoteIR0notfound = 1;
                    if (WiimoteIR0notfound == 1 & !WiimoteIR0found & !WiimoteIR1found)
                        WiimoteIR0notfound = 2;
                    if (WiimoteIR0notfound == 2 & WiimoteIR0found)
                    {
                        WiimoteIR0notfound = 0;
                        if (!WiimoteIRswitch)
                            WiimoteIRswitch = true;
                        else
                            WiimoteIRswitch = false;
                    }
                    if (WiimoteIR0notfound == 0 & WiimoteIR0found)
                        WiimoteIR0notfound = 0;
                    if (WiimoteIR0notfound == 0 & !WiimoteIR0found & !WiimoteIR1found)
                        WiimoteIR0notfound = 0;
                    if (WiimoteIR0notfound == 1 & WiimoteIR0found)
                        WiimoteIR0notfound = 0;
                    if (WiimoteIR0found)
                    {
                        WiimoteIRSensors0X = (aBuffer[6] | ((aBuffer[8] >> 4) & 0x03) << 8);
                        WiimoteIRSensors0Y = (aBuffer[7] | ((aBuffer[8] >> 6) & 0x03) << 8);
                    }
                    if (WiimoteIR1found)
                    {
                        WiimoteIRSensors1X = (aBuffer[9] | ((aBuffer[8] >> 0) & 0x03) << 8);
                        WiimoteIRSensors1Y = (aBuffer[10] | ((aBuffer[8] >> 2) & 0x03) << 8);
                    }
                    if (WiimoteIRswitch)
                    {
                        WiimoteIR0foundcam = WiimoteIR0found;
                        WiimoteIR1foundcam = WiimoteIR1found;
                        WiimoteIRSensors0Xcam = WiimoteIRSensors0X - 512f;
                        WiimoteIRSensors0Ycam = WiimoteIRSensors0Y - 384f;
                        WiimoteIRSensors1Xcam = WiimoteIRSensors1X - 512f;
                        WiimoteIRSensors1Ycam = WiimoteIRSensors1Y - 384f;
                    }
                    else
                    {
                        WiimoteIR1foundcam = WiimoteIR0found;
                        WiimoteIR0foundcam = WiimoteIR1found;
                        WiimoteIRSensors1Xcam = WiimoteIRSensors0X - 512f;
                        WiimoteIRSensors1Ycam = WiimoteIRSensors0Y - 384f;
                        WiimoteIRSensors0Xcam = WiimoteIRSensors1X - 512f;
                        WiimoteIRSensors0Ycam = WiimoteIRSensors1Y - 384f;
                    }
                    if (WiimoteIR0foundcam & WiimoteIR1foundcam)
                    {
                        irx2 = WiimoteIRSensors0Xcam;
                        iry2 = WiimoteIRSensors0Ycam;
                        irx3 = WiimoteIRSensors1Xcam;
                        iry3 = WiimoteIRSensors1Ycam;
                        WiimoteIRSensorsXcam = WiimoteIRSensors0Xcam - WiimoteIRSensors1Xcam;
                        WiimoteIRSensorsYcam = WiimoteIRSensors0Ycam - WiimoteIRSensors1Ycam;
                    }
                    if (WiimoteIR0foundcam & !WiimoteIR1foundcam)
                    {
                        irx2 = WiimoteIRSensors0Xcam;
                        iry2 = WiimoteIRSensors0Ycam;
                        irx3 = WiimoteIRSensors0Xcam - WiimoteIRSensorsXcam;
                        iry3 = WiimoteIRSensors0Ycam - WiimoteIRSensorsYcam;
                    }
                    if (WiimoteIR1foundcam & !WiimoteIR0foundcam)
                    {
                        irx3 = WiimoteIRSensors1Xcam;
                        iry3 = WiimoteIRSensors1Ycam;
                        irx2 = WiimoteIRSensors1Xcam + WiimoteIRSensorsXcam;
                        iry2 = WiimoteIRSensors1Ycam + WiimoteIRSensorsYcam;
                    }
                    irxc = irx2 + irx3;
                    iryc = iry2 + iry3;
                }
                irx = irxc * (1024f / 1346f);
                iry = iryc + centery >= 0 ? Scale(iryc + centery, 0f, 782f + centery, 0f, 1024f) : Scale(iryc + centery, -782f + centery, 0f, -1024f, 0f);
                WiimoteButtonStateA = (aBuffer[2] & 0x08) != 0;
                WiimoteButtonStateB = (aBuffer[2] & 0x04) != 0;
                WiimoteButtonStateMinus = (aBuffer[2] & 0x10) != 0;
                WiimoteButtonStateHome = (aBuffer[2] & 0x80) != 0;
                WiimoteButtonStatePlus = (aBuffer[1] & 0x10) != 0;
                WiimoteButtonStateOne = (aBuffer[2] & 0x02) != 0;
                WiimoteButtonStateTwo = (aBuffer[2] & 0x01) != 0;
                WiimoteButtonStateUp = (aBuffer[1] & 0x08) != 0;
                WiimoteButtonStateDown = (aBuffer[1] & 0x04) != 0;
                WiimoteButtonStateLeft = (aBuffer[1] & 0x01) != 0;
                WiimoteButtonStateRight = (aBuffer[1] & 0x02) != 0;
                WiimoteRawValuesX = aBuffer[3] - 135f + calibrationinit;
                WiimoteRawValuesY = aBuffer[4] - 135f + calibrationinit;
                WiimoteRawValuesZ = aBuffer[5] - 135f + calibrationinit;
                WiimoteButtonACC = (WiimoteRawValuesZ > 0 ? WiimoteRawValuesZ : -WiimoteRawValuesZ) >= 30f & (WiimoteRawValuesY > 0 ? WiimoteRawValuesY : -WiimoteRawValuesY) >= 30f & (WiimoteRawValuesX > 0 ? WiimoteRawValuesX : -WiimoteRawValuesX) >= 30f;
                stick_rawLeft[0] = report_bufLeft[6 + (ISLEFT ? 0 : 3)];
                stick_rawLeft[1] = report_bufLeft[7 + (ISLEFT ? 0 : 3)];
                stick_rawLeft[2] = report_bufLeft[8 + (ISLEFT ? 0 : 3)];
                stickLeft[0] = ((UInt16)(stick_rawLeft[0] | ((stick_rawLeft[1] & 0xf) << 8)) - stickCenterLeft[0]) / 1440f;
                stickLeft[1] = ((UInt16)((stick_rawLeft[1] >> 4) | (stick_rawLeft[2] << 4)) - stickCenterLeft[1]) / 1440f;
                JoyconLeftStickX = stickLeft[0];
                JoyconLeftStickY = stickLeft[1];
                acc_gLeft.X = ((Int16)(report_bufLeft[13 + 0 * 12] | ((report_bufLeft[14 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufLeft[13 + 1 * 12] | ((report_bufLeft[14 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufLeft[13 + 2 * 12] | ((report_bufLeft[14 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationLeftX) * (1.0f / 12000f);
                acc_gLeft.Y = -((Int16)(report_bufLeft[15 + 0 * 12] | ((report_bufLeft[16 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufLeft[15 + 1 * 12] | ((report_bufLeft[16 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufLeft[15 + 2 * 12] | ((report_bufLeft[16 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationLeftY) * (1.0f / 12000f);
                acc_gLeft.Z = -((Int16)(report_bufLeft[17 + 0 * 12] | ((report_bufLeft[18 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufLeft[17 + 1 * 12] | ((report_bufLeft[18 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufLeft[17 + 2 * 12] | ((report_bufLeft[18 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationLeftZ) * (1.0f / 12000f);
                JoyconLeftButtonSHOULDER_1 = (report_bufLeft[3 + (ISLEFT ? 2 : 0)] & 0x40) != 0;
                JoyconLeftButtonSHOULDER_2 = (report_bufLeft[3 + (ISLEFT ? 2 : 0)] & 0x80) != 0;
                JoyconLeftButtonSR = (report_bufLeft[3 + (ISLEFT ? 2 : 0)] & 0x10) != 0;
                JoyconLeftButtonSL = (report_bufLeft[3 + (ISLEFT ? 2 : 0)] & 0x20) != 0;
                JoyconLeftButtonDPAD_DOWN = (report_bufLeft[3 + (ISLEFT ? 2 : 0)] & (ISLEFT ? 0x01 : 0x04)) != 0;
                JoyconLeftButtonDPAD_RIGHT = (report_bufLeft[3 + (ISLEFT ? 2 : 0)] & (ISLEFT ? 0x04 : 0x08)) != 0;
                JoyconLeftButtonDPAD_UP = (report_bufLeft[3 + (ISLEFT ? 2 : 0)] & (ISLEFT ? 0x02 : 0x02)) != 0;
                JoyconLeftButtonDPAD_LEFT = (report_bufLeft[3 + (ISLEFT ? 2 : 0)] & (ISLEFT ? 0x08 : 0x01)) != 0;
                JoyconLeftButtonMINUS = (report_bufLeft[4] & 0x01) != 0;
                JoyconLeftButtonCAPTURE = (report_bufLeft[4] & 0x20) != 0;
                JoyconLeftButtonSTICK = (report_bufLeft[4] & (ISLEFT ? 0x08 : 0x04)) != 0;
                JoyconLeftButtonACC = acc_gLeft.X <= -1.13;
                JoyconLeftButtonSMA = JoyconLeftButtonSL | JoyconLeftButtonSR | JoyconLeftButtonMINUS | JoyconLeftButtonACC;
                if (LeftValListY.Count >= 50)
                {
                    LeftValListY.RemoveAt(0);
                    LeftValListY.Add(acc_gLeft.Y);
                }
                else
                    LeftValListY.Add(acc_gLeft.Y);
                JoyconLeftRollLeft = LeftValListY.Average() <= -0.75f;
                JoyconLeftRollRight = LeftValListY.Average() >= 0.75f;
                DirectAnglesLeft = acc_gLeft - InitDirectAnglesLeft;
                JoyconLeftButtonACC = acc_gLeft.X <= -1.13;
                JoyconLeftAccelX = DirectAnglesLeft.X * 1350f;
                JoyconLeftAccelY = -DirectAnglesLeft.Y * 1350f;
            }
        }
        public static void taskX()
        {
            if (enabling)
            {
                if (irx >= 0f & irx <= 1024f)
                    mousex = Scale(irx * irx * irx / 1024f / 1024f * viewpower3x + irx * irx / 1024f * viewpower2x + irx * viewpower1x, 0f, 1024f, (dzx / 100f) * 1024f, 1024f);
                if (irx <= 0f & irx >= -1024f)
                    mousex = Scale(-(-irx * -irx * -irx) / 1024f / 1024f * viewpower3x - (-irx * -irx) / 1024f * viewpower2x - (-irx) * viewpower1x, -1024f, 0f, -1024f, -(dzx / 100f) * 1024f);
                if (iry >= 0f & iry <= 1024f)
                    mousey = Scale(iry * iry * iry / 1024f / 1024f * viewpower3y + iry * iry / 1024f * viewpower2y + iry * viewpower1y, 0f, 1024f, (dzy / 100f) * 1024f, 1024f);
                if (iry <= 0f & iry >= -1024f)
                    mousey = Scale(-(-iry * -iry * -iry) / 1024f / 1024f * viewpower3y - (-iry * -iry) / 1024f * viewpower2y - (-iry) * viewpower1y, -1024f, 0f, -1024f, -(dzy / 100f) * 1024f);
                mousexp = mousex * irxsens;
                mouseyp = mousey * irysens;
                rightstickx = Math.Abs(-mousexp / 1024f * 32767f) <= 32767f ? -mousexp / 1024f * 32767f : Math.Sign(mousexp) * 32767f;
                rightsticky = Math.Abs(-mouseyp / 1024f * 32767f) <= 32767f ? -mouseyp / 1024f * 32767f : Math.Sign(mouseyp) * 32767f;
                if (movesmooth)
                {
                    mousexp = -JoyconLeftStickX * 1400f * stickxsens;
                    mouseyp = -JoyconLeftStickY * 1400f * stickysens;
                    leftstickx = Math.Abs(-mousexp * 32767f / 1024f) <= 32767f ? -mousexp * 32767f / 1024f : Math.Sign(-mousexp) * 32767f;
                    leftsticky = Math.Abs(-mouseyp * 32767f / 1024f) <= 32767f ? -mouseyp * 32767f / 1024f : Math.Sign(-mouseyp) * 32767f;
                }
                else
                {
                    if (JoyconLeftStickX > 0.35f / stickxsens)
                        leftstickx = 32767;
                    if (JoyconLeftStickX < -0.35f / stickxsens)
                        leftstickx = -32767;
                    if (JoyconLeftStickX <= 0.35f / stickxsens & JoyconLeftStickX >= -0.35f / stickxsens)
                        leftstickx = 0;
                    if (JoyconLeftStickY > 0.35f / stickysens)
                        leftsticky = 32767;
                    if (JoyconLeftStickY < -0.35f / stickysens)
                        leftsticky = -32767;
                    if (JoyconLeftStickY <= 0.35f / stickysens & JoyconLeftStickY >= -0.35f / stickysens)
                        leftsticky = 0;
                }
                if (scriptenabling)
                {
                    object[] val = (object[])program.InvokeMember("Main", BindingFlags.Default | BindingFlags.InvokeMethod, null, obj, new object[] { adsswitch, JoyconLeftButtonDPAD_DOWN, JoyconLeftButtonDPAD_LEFT, JoyconLeftButtonDPAD_RIGHT, JoyconLeftButtonDPAD_UP, JoyconLeftButtonMINUS, JoyconLeftButtonACC, JoyconLeftButtonSHOULDER_1, JoyconLeftButtonSHOULDER_2, JoyconLeftButtonCAPTURE, WiimoteButtonStateOne, WiimoteButtonStateTwo, WiimoteButtonStateDown, WiimoteButtonStateLeft, WiimoteButtonStateRight, WiimoteButtonStateUp, WiimoteButtonStateHome, WiimoteButtonACC, WiimoteButtonStateA, WiimoteButtonStateB, WiimoteButtonStatePlus, WiimoteButtonStateMinus });
                    back = (bool)val[0]; start = (bool)val[1]; A = (bool)val[2]; B = (bool)val[3]; X = (bool)val[4]; Y = (bool)val[5]; up = (bool)val[6]; left = (bool)val[7]; down = (bool)val[8]; right = (bool)val[9]; leftstick = (bool)val[10]; rightstick = (bool)val[11]; leftbumper = (bool)val[12]; rightbumper = (bool)val[13]; lefttrigger = (bool)val[14]; righttrigger = (bool)val[15];
                }
                ScpBus.SetController(back, start, A, B, X, Y, up, left, down, right, leftstick, rightstick, leftbumper, rightbumper, lefttrigger, righttrigger, leftstickx, leftsticky, rightstickx, rightsticky);
            }
        }
        private static double Scale(double value, double min, double max, double minScale, double maxScale)
        {
            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
        private static void taskD()
        {
            while (running)
            {
                try
                {
                    mStream.Read(aBuffer, 0, 22);
                }
                catch { }
            }
        }
        private static void taskDLeft()
        {
            while (running)
            {
                try
                {
                    Lhid_read_timeout(handleLeft, report_bufLeft, (UIntPtr)report_lenLeft);
                }
                catch { }
            }
        }
        public static void Close()
        {
            try
            {
                running = false;
                Thread.Sleep(100);
                ScpBus.UnLoadController();
                mStream.Close();
                handle.Close();
                Subcommand3Left(0x06, new byte[] { 0x01 }, 1);
                Lhid_close(handleLeft);
                handleLeft.Close();
                wiimotedisconnect();
                joyconleftdisconnect();
            }
            catch { }
        }
        private const string vendor_id = "57e", vendor_id_ = "057e", product_r1 = "0330", product_r2 = "0306", product_l = "2006";
        private enum EFileAttributes : uint
        {
            Overlapped = 0x40000000,
            Normal = 0x80
        };
        struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid InterfaceClassGuid;
            public int Flags;
            public IntPtr RESERVED;
        }
        struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public UInt32 cbSize;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }
        [DllImport("MotionInputPairing.dll", EntryPoint = "wiimotejoyconleftconnect")]
        public static extern bool wiimotejoyconleftconnect();
        [DllImport("MotionInputPairing.dll", EntryPoint = "joyconleftdisconnect")]
        public static extern bool joyconleftdisconnect();
        [DllImport("MotionInputPairing.dll", EntryPoint = "wiimotedisconnect")]
        public static extern bool wiimotedisconnect();
        private static bool ScanScanLeft()
        {
            ISWIIMOTE = false;
            ISLEFT = false;
            int index = 0;
            System.Guid guid;
            HidD_GetHidGuid(out guid);
            System.IntPtr hDevInfo = SetupDiGetClassDevs(ref guid, null, new System.IntPtr(), 0x00000010);
            SP_DEVICE_INTERFACE_DATA diData = new SP_DEVICE_INTERFACE_DATA();
            diData.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(diData);
            while (SetupDiEnumDeviceInterfaces(hDevInfo, new System.IntPtr(), ref guid, index, ref diData))
            {
                System.UInt32 size;
                SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, new System.IntPtr(), 0, out size, new System.IntPtr());
                SP_DEVICE_INTERFACE_DETAIL_DATA diDetail = new SP_DEVICE_INTERFACE_DETAIL_DATA();
                diDetail.cbSize = 5;
                if (SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, ref diDetail, size, out size, new System.IntPtr()))
                {
                    if ((diDetail.DevicePath.Contains(vendor_id) | diDetail.DevicePath.Contains(vendor_id_)) & (diDetail.DevicePath.Contains(product_r1) | diDetail.DevicePath.Contains(product_r2)))
                    {
                        ISWIIMOTE = true;
                        WiimoteFound(diDetail.DevicePath);
                    }
                    if ((diDetail.DevicePath.Contains(vendor_id) | diDetail.DevicePath.Contains(vendor_id_)) & diDetail.DevicePath.Contains(product_l))
                    {
                        ISLEFT = true;
                        AttachJoyLeft(diDetail.DevicePath);
                    }
                    if (ISWIIMOTE & ISLEFT)
                        return true;
                }
                index++;
            }
            return false;
        }
        private static void WiimoteFound(string path)
        {
            do
            {
                handle = CreateFile(path, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, (uint)EFileAttributes.Overlapped, IntPtr.Zero);
                WriteData(handle, IR, (int)REGISTER_IR, new byte[] { 0x08 }, 1);
                WriteData(handle, Type, (int)REGISTER_EXTENSION_INIT_1, new byte[] { 0x55 }, 1);
                WriteData(handle, Type, (int)REGISTER_EXTENSION_INIT_2, new byte[] { 0x00 }, 1);
                WriteData(handle, Type, (int)REGISTER_MOTIONPLUS_INIT, new byte[] { 0x04 }, 1);
                ReadData(handle, 0x0016, 7);
                ReadData(handle, (int)REGISTER_EXTENSION_TYPE, 6);
                ReadData(handle, (int)REGISTER_EXTENSION_CALIBRATION, 16);
                ReadData(handle, (int)REGISTER_EXTENSION_CALIBRATION, 32);
            }
            while (handle.IsInvalid);
            mStream = new FileStream(handle, FileAccess.ReadWrite, 22, true);
        }
        private static void ReadData(SafeFileHandle _hFile, int address, short size)
        {
            mBuff[0] = (byte)ReadMemory;
            mBuff[1] = (byte)((address & 0xff000000) >> 24);
            mBuff[2] = (byte)((address & 0x00ff0000) >> 16);
            mBuff[3] = (byte)((address & 0x0000ff00) >> 8);
            mBuff[4] = (byte)(address & 0x000000ff);
            mBuff[5] = (byte)((size & 0xff00) >> 8);
            mBuff[6] = (byte)(size & 0xff);
            HidD_SetOutputReport(_hFile.DangerousGetHandle(), mBuff, 22);
        }
        private static void WriteData(SafeFileHandle _hFile, byte mbuff, int address, byte[] buff, short size)
        {
            mBuff[0] = (byte)mbuff;
            mBuff[1] = (byte)(0x04);
            mBuff[2] = (byte)IRExtensionAccel;
            Array.Copy(buff, 0, mBuff, 3, 1);
            HidD_SetOutputReport(_hFile.DangerousGetHandle(), mBuff, 22);
            mBuff[0] = (byte)WriteMemory;
            mBuff[1] = (byte)(((address & 0xff000000) >> 24));
            mBuff[2] = (byte)((address & 0x00ff0000) >> 16);
            mBuff[3] = (byte)((address & 0x0000ff00) >> 8);
            mBuff[4] = (byte)((address & 0x000000ff) >> 0);
            mBuff[5] = (byte)size;
            Array.Copy(buff, 0, mBuff, 6, 1);
            HidD_SetOutputReport(_hFile.DangerousGetHandle(), mBuff, 22);
        }
        public static bool JoyconLeftButtonSMA, JoyconLeftButtonACC, JoyconLeftRollLeft, JoyconLeftRollRight;
        private static double JoyconLeftStickX, JoyconLeftStickY;
        public static System.Collections.Generic.List<double> LeftValListX = new System.Collections.Generic.List<double>(), LeftValListY = new System.Collections.Generic.List<double>();
        public static bool JoyconLeftGyroCenter, JoyconLeftAccelCenter;
        public static double JoyconLeftAccelX, JoyconLeftAccelY, JoyconLeftGyroX, JoyconLeftGyroY;
        public static Vector3 InitEulerAnglesaLeft, EulerAnglesaLeft, InitEulerAnglesbLeft, EulerAnglesbLeft;
        public static Vector3 gyr_gLeft = new Vector3();
        public static Vector3 i_aLeft = new Vector3(1, 0, 0);
        public static Vector3 j_aLeft = new Vector3(0, 1, 0);
        public static Vector3 k_aLeft = new Vector3(0, 0, 1);
        public static Vector3 k_aCrossLeft = new Vector3(0, 0, 1);
        public static Vector3 i_bLeft = new Vector3(1, 0, 0);
        public static Vector3 j_bLeft = new Vector3(0, 1, 0);
        public static Vector3 j_bCrossLeft = new Vector3(0, 1, 0);
        public static Vector3 k_bLeft = new Vector3(0, 0, 1);
        private static double[] stickLeft = { 0, 0 };
        private static double[] stickCenterLeft = { 0, 0 };
        private static byte[] stick_rawLeft = { 0, 0, 0 };
        public static SafeFileHandle handleLeft;
        public static Vector3 acc_gLeft = new Vector3();
        public const uint report_lenLeft = 49;
        public static Vector3 InitDirectAnglesLeft, DirectAnglesLeft;
        public static bool JoyconLeftButtonSHOULDER_1, JoyconLeftButtonSHOULDER_2, JoyconLeftButtonSR, JoyconLeftButtonSL, JoyconLeftButtonDPAD_DOWN, JoyconLeftButtonDPAD_RIGHT, JoyconLeftButtonDPAD_UP, JoyconLeftButtonDPAD_LEFT, JoyconLeftButtonMINUS, JoyconLeftButtonSTICK, JoyconLeftButtonCAPTURE, ISLEFT;
        public static byte[] report_bufLeft = new byte[report_lenLeft];
        public static float acc_gcalibrationLeftX, acc_gcalibrationLeftY, acc_gcalibrationLeftZ;
        public static void AttachJoyLeft(string path)
        {
            do
            {
                IntPtr handle = CreateFile(path, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite, new System.IntPtr(), System.IO.FileMode.Open, EFileAttributes.Normal, new System.IntPtr());
                handleLeft = Lhid_open_path(handle);
                Subcommand2Left(0x40, new byte[] { 0x1 }, 1);
                Subcommand2Left(0x3, new byte[] { 0x30 }, 1);
            }
            while (handleLeft.IsInvalid);
        }
        private static void Subcommand2Left(byte sc, byte[] buf, uint len)
        {
            byte[] buf_Left = new byte[report_lenLeft];
            System.Array.Copy(buf, 0, buf_Left, 11, len);
            buf_Left[10] = sc;
            buf_Left[1] = 0;
            buf_Left[0] = 0x1;
            Lhid_write(handleLeft, buf_Left, (UIntPtr)(len + 11));
            Lhid_read_timeout(handleLeft, buf_Left, (UIntPtr)report_lenLeft);
        }
        private static void Subcommand3Left(byte sc, byte[] buf, uint len)
        {
            byte[] buf_Left = new byte[report_lenLeft];
            System.Array.Copy(buf, 0, buf_Left, 11, len);
            buf_Left[10] = sc;
            buf_Left[1] = 0x5;
            buf_Left[0] = 0x80;
            Lhid_write(handleLeft, buf_Left, new UIntPtr(2));
            buf_Left[1] = 0x6;
            buf_Left[0] = 0x80;
            Lhid_write(handleLeft, buf_Left, new UIntPtr(2));
        }
    }
    [System.Security.SuppressUnmanagedCodeSecurity]
    public class ScpBus : IDisposable
    {
        public static int[] wd = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
        public static int[] wu = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
        public static void valchanged(int n, bool val)
        {
            if (val)
            {
                if (wd[n] <= 1)
                {
                    wd[n] = wd[n] + 1;
                }
                wu[n] = 0;
            }
            else
            {
                if (wu[n] <= 1)
                {
                    wu[n] = wu[n] + 1;
                }
                wd[n] = 0;
            }
        }
        private static ScpBus scpBus;
        private static X360Controller controller;
        public static void LoadController()
        {
            scpBus = new ScpBus();
            scpBus.PlugIn(1);
            controller = new X360Controller();
        }
        public static void UnLoadController()
        {
            SetController(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, 0, 0, 0, 0);
            Thread.Sleep(100);
            scpBus.Unplug(1);
        }
        public static void SetController(bool back, bool start, bool A, bool B, bool X, bool Y, bool up, bool left, bool down, bool right, bool leftstick, bool rightstick, bool leftbumper, bool rightbumper, bool lefttrigger, bool righttrigger, double leftstickx, double leftsticky, double rightstickx, double rightsticky)
        {
            valchanged(1, back);
            if (wd[1] == 1)
                controller.Buttons ^= X360Buttons.Back;
            if (wu[1] == 1)
                controller.Buttons &= ~X360Buttons.Back;
            valchanged(2, start);
            if (wd[2] == 1)
                controller.Buttons ^= X360Buttons.Start;
            if (wu[2] == 1)
                controller.Buttons &= ~X360Buttons.Start;
            valchanged(3, A);
            if (wd[3] == 1)
                controller.Buttons ^= X360Buttons.A;
            if (wu[3] == 1)
                controller.Buttons &= ~X360Buttons.A;
            valchanged(4, B);
            if (wd[4] == 1)
                controller.Buttons ^= X360Buttons.B;
            if (wu[4] == 1)
                controller.Buttons &= ~X360Buttons.B;
            valchanged(5, X);
            if (wd[5] == 1)
                controller.Buttons ^= X360Buttons.X;
            if (wu[5] == 1)
                controller.Buttons &= ~X360Buttons.X;
            valchanged(6, Y);
            if (wd[6] == 1)
                controller.Buttons ^= X360Buttons.Y;
            if (wu[6] == 1)
                controller.Buttons &= ~X360Buttons.Y;
            valchanged(7, up);
            if (wd[7] == 1)
                controller.Buttons ^= X360Buttons.Up;
            if (wu[7] == 1)
                controller.Buttons &= ~X360Buttons.Up;
            valchanged(8, left);
            if (wd[8] == 1)
                controller.Buttons ^= X360Buttons.Left;
            if (wu[8] == 1)
                controller.Buttons &= ~X360Buttons.Left;
            valchanged(9, down);
            if (wd[9] == 1)
                controller.Buttons ^= X360Buttons.Down;
            if (wu[9] == 1)
                controller.Buttons &= ~X360Buttons.Down;
            valchanged(10, right);
            if (wd[10] == 1)
                controller.Buttons ^= X360Buttons.Right;
            if (wu[10] == 1)
                controller.Buttons &= ~X360Buttons.Right;
            valchanged(11, leftstick);
            if (wd[11] == 1)
                controller.Buttons ^= X360Buttons.LeftStick;
            if (wu[11] == 1)
                controller.Buttons &= ~X360Buttons.LeftStick;
            valchanged(12, rightstick);
            if (wd[12] == 1)
                controller.Buttons ^= X360Buttons.RightStick;
            if (wu[12] == 1)
                controller.Buttons &= ~X360Buttons.RightStick;
            valchanged(13, leftbumper);
            if (wd[13] == 1)
                controller.Buttons ^= X360Buttons.LeftBumper;
            if (wu[13] == 1)
                controller.Buttons &= ~X360Buttons.LeftBumper;
            valchanged(14, rightbumper);
            if (wd[14] == 1)
                controller.Buttons ^= X360Buttons.RightBumper;
            if (wu[14] == 1)
                controller.Buttons &= ~X360Buttons.RightBumper;
            if (lefttrigger)
                controller.LeftTrigger = 255;
            else
                controller.LeftTrigger = 0;
            if (righttrigger)
                controller.RightTrigger = 255;
            else
                controller.RightTrigger = 0;
            controller.LeftStickX = (short)leftstickx;
            controller.LeftStickY = (short)leftsticky;
            controller.RightStickX = (short)rightstickx;
            controller.RightStickY = (short)rightsticky;
            scpBus.Report(controller.GetReport());
        }
        private const string SCP_BUS_CLASS_GUID = "{F679F562-3164-42CE-A4DB-E7DDBE723909}";
        private const int ReportSize = 28;

        private readonly SafeFileHandle _deviceHandle;

        /// <summary>
        /// Creates a new ScpBus object, which will then try to get a handle to the SCP Virtual Bus device. If it is unable to get the handle, an IOException will be thrown.
        /// </summary>
        public ScpBus() : this(0) { }

        /// <summary>
        /// Creates a new ScpBus object, which will then try to get a handle to the SCP Virtual Bus device. If it is unable to get the handle, an IOException will be thrown.
        /// </summary>
        /// <param name="instance">Specifies which SCP Virtual Bus device to use. This is 0-based.</param>
        public ScpBus(int instance)
        {
            string devicePath = "";

            if (Find(new Guid(SCP_BUS_CLASS_GUID), ref devicePath, instance))
            {
                _deviceHandle = GetHandle(devicePath);
            }
            else
            {
                throw new IOException("SCP Virtual Bus Device not found");
            }
        }

        /// <summary>
        /// Creates a new ScpBus object, which will then try to get a handle to the specified SCP Virtual Bus device. If it is unable to get the handle, an IOException will be thrown.
        /// </summary>
        /// <param name="devicePath">The path to the SCP Virtual Bus device that you want to use.</param>
        public ScpBus(string devicePath)
        {
            _deviceHandle = GetHandle(devicePath);
        }

        /// <summary>
        /// Closes the handle to the SCP Virtual Bus device. Call this when you are done with your instance of ScpBus.
        /// 
        /// (This method does the same thing as the Dispose() method. Use one or the other.)
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Closes the handle to the SCP Virtual Bus device. Call this when you are done with your instance of ScpBus.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Internal disposer, called by either the finalizer or the Dispose() method.
        /// </summary>
        /// <param name="disposing">True if called from Dispose(), false if called from finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_deviceHandle != null && !_deviceHandle.IsInvalid)
            {
                _deviceHandle.Dispose();
            }
        }

        /// <summary>
        /// Plugs in an emulated Xbox 360 controller.
        /// </summary>
        /// <param name="controllerNumber">Used to identify the controller. Give each controller you plug in a different number. Number must be non-zero.</param>
        /// <returns>True if the operation was successful, false otherwise.</returns>
        public bool PlugIn(int controllerNumber)
        {
            if (_deviceHandle.IsInvalid)
                throw new ObjectDisposedException("SCP Virtual Bus device handle is closed");

            int transfered = 0;
            byte[] buffer = new byte[16];

            buffer[0] = 0x10;
            buffer[1] = 0x00;
            buffer[2] = 0x00;
            buffer[3] = 0x00;

            buffer[4] = (byte)((controllerNumber) & 0xFF);
            buffer[5] = (byte)((controllerNumber >> 8) & 0xFF);
            buffer[6] = (byte)((controllerNumber >> 16) & 0xFF);
            buffer[7] = (byte)((controllerNumber >> 24) & 0xFF);

            return NativeMethods.DeviceIoControl(_deviceHandle, 0x2A4000, buffer, buffer.Length, null, 0, ref transfered, IntPtr.Zero);
        }

        /// <summary>
        /// Unplugs an emulated Xbox 360 controller.
        /// </summary>
        /// <param name="controllerNumber">The controller you want to unplug.</param>
        /// <returns>True if the operation was successful, false otherwise.</returns>
        public bool Unplug(int controllerNumber)
        {
            if (_deviceHandle.IsInvalid)
                throw new ObjectDisposedException("SCP Virtual Bus device handle is closed");

            int transfered = 0;
            byte[] buffer = new Byte[16];

            buffer[0] = 0x10;
            buffer[1] = 0x00;
            buffer[2] = 0x00;
            buffer[3] = 0x00;

            buffer[4] = (byte)((controllerNumber) & 0xFF);
            buffer[5] = (byte)((controllerNumber >> 8) & 0xFF);
            buffer[6] = (byte)((controllerNumber >> 16) & 0xFF);
            buffer[7] = (byte)((controllerNumber >> 24) & 0xFF);

            return NativeMethods.DeviceIoControl(_deviceHandle, 0x2A4004, buffer, buffer.Length, null, 0, ref transfered, IntPtr.Zero);
        }

        /// <summary>
        /// Unplugs all emulated Xbox 360 controllers.
        /// </summary>
        /// <returns>True if the operation was successful, false otherwise.</returns>
        public bool UnplugAll()
        {
            if (_deviceHandle.IsInvalid)
                throw new ObjectDisposedException("SCP Virtual Bus device handle is closed");

            int transfered = 0;
            byte[] buffer = new byte[16];

            buffer[0] = 0x10;
            buffer[1] = 0x00;
            buffer[2] = 0x00;
            buffer[3] = 0x00;

            return NativeMethods.DeviceIoControl(_deviceHandle, 0x2A4004, buffer, buffer.Length, null, 0, ref transfered, IntPtr.Zero);
        }
        int transferred;
        byte[] outputBuffer = null;
        /// <summary>
        /// Sends an input report for the current state of the specified emulated Xbox 360 controller. Note: Only use this if you don't care about rumble data, otherwise use the 3-parameter version of Report().
        /// </summary>
        /// <param name="controllerNumber">The controller to report.</param>
        /// <param name="controllerReport">The controller report. If using the included X360Controller class, this can be generated with the GetReport() method. Otherwise see http://free60.org/wiki/GamePad#Input_report for details.</param>
        /// <returns>True if the operation was successful, false otherwise.</returns>
        public bool Report(byte[] controllerReport)
        {
            return NativeMethods.DeviceIoControl(_deviceHandle, 0x2A400C, controllerReport, controllerReport.Length, outputBuffer, outputBuffer?.Length ?? 0, ref transferred, IntPtr.Zero);
        }

        private static bool Find(Guid target, ref string path, int instance = 0)
        {
            IntPtr detailDataBuffer = IntPtr.Zero;
            IntPtr deviceInfoSet = IntPtr.Zero;

            try
            {
                NativeMethods.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = new NativeMethods.SP_DEVICE_INTERFACE_DATA(), da = new NativeMethods.SP_DEVICE_INTERFACE_DATA();
                int bufferSize = 0, memberIndex = 0;

                deviceInfoSet = NativeMethods.SetupDiGetClassDevs(ref target, IntPtr.Zero, IntPtr.Zero, NativeMethods.DIGCF_PRESENT | NativeMethods.DIGCF_DEVICEINTERFACE);

                DeviceInterfaceData.cbSize = da.cbSize = Marshal.SizeOf(DeviceInterfaceData);

                while (NativeMethods.SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref target, memberIndex, ref DeviceInterfaceData))
                {
                    NativeMethods.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref DeviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, ref da);
                    detailDataBuffer = Marshal.AllocHGlobal(bufferSize);

                    Marshal.WriteInt32(detailDataBuffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);

                    if (NativeMethods.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref DeviceInterfaceData, detailDataBuffer, bufferSize, ref bufferSize, ref da))
                    {
                        IntPtr pDevicePathName = detailDataBuffer + 4;

                        path = Marshal.PtrToStringAuto(pDevicePathName).ToUpper(CultureInfo.InvariantCulture);
                        Marshal.FreeHGlobal(detailDataBuffer);

                        if (memberIndex == instance) return true;
                    }
                    else Marshal.FreeHGlobal(detailDataBuffer);


                    memberIndex++;
                }
            }
            finally
            {
                if (deviceInfoSet != IntPtr.Zero)
                {
                    NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }
            }

            return false;
        }

        private static SafeFileHandle GetHandle(string devicePath)
        {
            devicePath = devicePath.ToUpper(CultureInfo.InvariantCulture);

            SafeFileHandle handle = NativeMethods.CreateFile(devicePath, (NativeMethods.GENERIC_WRITE | NativeMethods.GENERIC_READ), NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, NativeMethods.FILE_ATTRIBUTE_NORMAL | NativeMethods.FILE_FLAG_OVERLAPPED, UIntPtr.Zero);

            if (handle == null || handle.IsInvalid)
            {
                throw new IOException("Unable to get SCP Virtual Bus Device handle");
            }

            return handle;
        }
    }

    [System.Security.SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct SP_DEVICE_INTERFACE_DATA
        {
            internal int cbSize;
            internal Guid InterfaceClassGuid;
            internal int Flags;
            internal IntPtr Reserved;
        }

        internal const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        internal const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        internal const uint FILE_SHARE_READ = 1;
        internal const uint FILE_SHARE_WRITE = 2;
        internal const uint GENERIC_READ = 0x80000000;
        internal const uint GENERIC_WRITE = 0x40000000;
        internal const uint OPEN_EXISTING = 3;
        internal const int DIGCF_PRESENT = 0x0002;
        internal const int DIGCF_DEVICEINTERFACE = 0x0010;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, UIntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeviceIoControl(SafeFileHandle hDevice, int dwIoControlCode, byte[] lpInBuffer, int nInBufferSize, byte[] lpOutBuffer, int nOutBufferSize, ref int lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern int SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        [DllImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInfo, ref Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, IntPtr enumerator, IntPtr hwndParent, int flags);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, ref SP_DEVICE_INTERFACE_DATA deviceInfoData);
    }
    /// <summary>
    /// A virtual Xbox 360 Controller. After setting the desired values, use the GetReport() method to generate a controller report that can be used with ScpBus's Report() method.
    /// </summary>
    [System.Security.SuppressUnmanagedCodeSecurity]
    public class X360Controller
    {
        /// <summary>
        /// Generates a new X360Controller object with the default initial state (no buttons pressed, all analog inputs 0).
        /// </summary>
        public X360Controller()
        {
            Buttons = X360Buttons.None;
            LeftTrigger = 0;
            RightTrigger = 0;
            LeftStickX = 0;
            LeftStickY = 0;
            RightStickX = 0;
            RightStickY = 0;
        }

        /// <summary>
        /// Generates a new X360Controller object. Optionally, you can specify the initial state of the controller.
        /// </summary>
        /// <param name="buttons">The pressed buttons. Use like flags (i.e. (X360Buttons.A | X360Buttons.X) would be mean both A and X are pressed).</param>
        /// <param name="leftTrigger">Left trigger analog input. 0 to 255.</param>
        /// <param name="rightTrigger">Right trigger analog input. 0 to 255.</param>
        /// <param name="leftStickX">Left stick X-axis. -32,768 to 32,767.</param>
        /// <param name="leftStickY">Left stick Y-axis. -32,768 to 32,767.</param>
        /// <param name="rightStickX">Right stick X-axis. -32,768 to 32,767.</param>
        /// <param name="rightStickY">Right stick Y-axis. -32,768 to 32,767.</param>
        public X360Controller(X360Buttons buttons, byte leftTrigger, byte rightTrigger, short leftStickX, short leftStickY, short rightStickX, short rightStickY)
        {
            Buttons = buttons;
            LeftTrigger = leftTrigger;
            RightTrigger = rightTrigger;
            LeftStickX = leftStickX;
            LeftStickY = leftStickY;
            RightStickX = rightStickX;
            RightStickY = rightStickY;
        }

        /// <summary>
        /// Generates a new X360Controller object with the same values as the specified X360Controller object.
        /// </summary>
        /// <param name="controller">An X360Controller object to copy values from.</param>
        public X360Controller(X360Controller controller)
        {
            Buttons = controller.Buttons;
            LeftTrigger = controller.LeftTrigger;
            RightTrigger = controller.RightTrigger;
            LeftStickX = controller.LeftStickX;
            LeftStickY = controller.LeftStickY;
            RightStickX = controller.RightStickX;
            RightStickY = controller.RightStickY;
        }

        /// <summary>
        /// The controller's currently pressed buttons. Use the X360Button values like flags (i.e. (X360Buttons.A | X360Buttons.X) would be mean both A and X are pressed).
        /// </summary>
        public X360Buttons Buttons { get; set; }

        /// <summary>
        /// The controller's left trigger analog input. Value can range from 0 to 255.
        /// </summary>
        public byte LeftTrigger { get; set; }

        /// <summary>
        /// The controller's right trigger analog input. Value can range from 0 to 255.
        /// </summary>
        public byte RightTrigger { get; set; }

        /// <summary>
        /// The controller's left stick X-axis. Value can range from -32,768 to 32,767.
        /// </summary>
        public short LeftStickX { get; set; }

        /// <summary>
        /// The controller's left stick Y-axis. Value can range from -32,768 to 32,767.
        /// </summary>
        public short LeftStickY { get; set; }

        /// <summary>
        /// The controller's right stick X-axis. Value can range from -32,768 to 32,767.
        /// </summary>
        public short RightStickX { get; set; }

        /// <summary>
        /// The controller's right stick Y-axis. Value can range from -32,768 to 32,767.
        /// </summary>
        public short RightStickY { get; set; }

        byte[] bytes = new byte[20];
        byte[] fullReport = { 0x1C, 0, 0, 0, (byte)((1) & 0xFF), (byte)((1 >> 8) & 0xFF), (byte)((1 >> 16) & 0xFF), (byte)((1 >> 24) & 0xFF), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        /// <summary>
        /// Generates an Xbox 360 controller report as specified here: http://free60.org/wiki/GamePad#Input_report. This can be used with ScpBus's Report() method.
        /// </summary>
        /// <returns>A 20-byte Xbox 360 controller report.</returns>
        public byte[] GetReport()
        {
            bytes[0] = 0x00;                                 // Message type (input report)
            bytes[1] = 0x14;                                 // Message size (20 bytes)

            bytes[2] = (byte)((ushort)Buttons & 0xFF);       // Buttons low
            bytes[3] = (byte)((ushort)Buttons >> 8 & 0xFF);  // Buttons high

            bytes[4] = LeftTrigger;                          // Left trigger
            bytes[5] = RightTrigger;                         // Right trigger

            bytes[6] = (byte)(LeftStickX & 0xFF);            // Left stick X-axis low
            bytes[7] = (byte)(LeftStickX >> 8 & 0xFF);       // Left stick X-axis high
            bytes[8] = (byte)(LeftStickY & 0xFF);            // Left stick Y-axis low
            bytes[9] = (byte)(LeftStickY >> 8 & 0xFF);       // Left stick Y-axis high

            bytes[10] = (byte)(RightStickX & 0xFF);          // Right stick X-axis low
            bytes[11] = (byte)(RightStickX >> 8 & 0xFF);     // Right stick X-axis high
            bytes[12] = (byte)(RightStickY & 0xFF);          // Right stick Y-axis low
            bytes[13] = (byte)(RightStickY >> 8 & 0xFF);     // Right stick Y-axis high

            // Remaining bytes are unused

            Array.Copy(bytes, 0, fullReport, 8, 20);

            return fullReport;
        }
    }

    /// <summary>
    /// The buttons to be used with an X360Controller object.
    /// </summary>
    [Flags]
    public enum X360Buttons
    {
        None = 0,

        Up = 1 << 0,
        Down = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,

        Start = 1 << 4,
        Back = 1 << 5,

        LeftStick = 1 << 6,
        RightStick = 1 << 7,

        LeftBumper = 1 << 8,
        RightBumper = 1 << 9,

        Logo = 1 << 10,

        A = 1 << 12,
        B = 1 << 13,
        X = 1 << 14,
        Y = 1 << 15,

    }
}