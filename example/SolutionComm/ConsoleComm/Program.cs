using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleComm
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitFlag = 0;
            while (true)
            {
                // 종료여부 체크
                if (exitFlag == 1) break;

                // 아이피 입력
                Console.WriteLine("아이피를 입력하세요.");
                Console.WriteLine("예시) 192.168.100.40");
                var ip = Console.ReadLine();

                Console.WriteLine("통신 시작");
                try
                {
                    var tcpClient = new TcpClient(ip, 502); //모드버스포트:502
                    var stream = tcpClient.GetStream();
                    stream.ReadTimeout = 1000;

                    // 전송 메세지
                    byte[] sendMsg = {
                    0x00, 0x00, //transaction ID: 고정
                    0x00, 0x00, //protocol ID: 고정
                    0x00, 0x06, //length: unit ID부터의 길이
                    0x01, //Unit ID, 특정값으로 고정하여 사용하여도 문제없음
                    0x04, //fuction code: Input Register:04, bit:02, Holding Register: 03, write multiple register: 06
                    0x00, 0x00, //start address: 시작주소
                    0x00, 0x02 //data count: 받을 데이터의 개수
                };

                    var strSendMsg = BitConverter.ToString(sendMsg);
                    Console.Write("전송: ");
                    Console.WriteLine(strSendMsg);

                    // 데이터 프레임
                    var dataCount = sendMsg[sendMsg.Length - 2] * 256 + sendMsg[sendMsg.Length - 1];
                    var functionCode = sendMsg[7];

                    // 받을 데이터의 길이 계산
                    var leng = 20;
                    switch (functionCode)
                    {
                        case 2:
                            leng = dataCount + 9;
                            break;
                        case 3:
                        case 4:
                            leng = 2 * dataCount + 9;
                            break;
                        default:
                            Console.WriteLine($"There is no function code called '{functionCode}'");
                            break;
                    }

                    // 데이터 전송
                    var receiveMsg = new byte[leng];
                    stream.Write(sendMsg, 0, sendMsg.Length);
                    stream.Read(receiveMsg, 0, receiveMsg.Length);

                    // 결과 출력
                    var result = BitConverter.ToString(receiveMsg);
                    Console.Write("수신: ");
                    Console.WriteLine(result);

                    //자원 해제
                    if (stream != null) stream.Close();
                    if (stream != null) stream.Dispose();
                    if (tcpClient != null) tcpClient.Close();
                    if (tcpClient != null) tcpClient.Dispose();
                }
                catch (Exception ex)
                {
                    // 에러출력
                    Console.WriteLine(ex);
                }

                // 종료여부 체크
                Console.WriteLine("종료하시겠습니까?");
                Console.WriteLine(" Yes: 1, No: 0");
                exitFlag = int.Parse(Console.ReadLine());
                Console.WriteLine();
            }
        }
    }
}
