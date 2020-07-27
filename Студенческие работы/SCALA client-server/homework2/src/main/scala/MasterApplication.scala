import java.io._
import java.net._
import scala.sys.process._
import scala.util._

object MasterApplication {

  var webPage=Array(
    "HTTP/1.1 200 OK\n"+"content-type: text/html;charset=utf-8\n"+"\"content-length:"
    ,
    "<head>"+
    "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"><meta http-equiv=\"Refresh\" content=\"8;URL=http:/"
    ,
      "/update\" />"+
    "</head>"+
    "<style>table{margin: 50px 0; text-align: left; border-collapse: separate; border: 1px solid #ddd; border-spacing: 10px; border-radius: 3px; background: #fdfdfd; font-size: 14px; width: auto; } td,th{ border: 1px solid #ddd; padding: 5px; border-radius: 3px; } th{ background: #E4E4E4; } caption{ font-style: bold; text-align: right; color: #000000; } </style>"+
    "<body>" +
    "<table><caption> Расчет числа Пи методом Монте-Карло </caption><tr><th> Количество точек </th><th> Время расчета </th><th> Значение </th></tr>"
  )

  val webPort = 8180
  val minSlavePort = 9000
  var slavePort = 9000
  val maxSlavePort = 9006

  def main(args: Array[String]): Unit = {
    new Thread(new Runnable() {
      override def run(): Unit = {
        while (true) {
          val webServSocket = new ServerSocket(webPort)
          //ожидание запроса браузера
          val webSocket = webServSocket.accept()
          println("Клиент найден")
          checkRequest(webServSocket,webSocket)
        }
      }
    }).start()
  }

  def checkRequest(webServSocket:ServerSocket,webSocket: Socket):String={
    var nrOfElements = 0
    val input = new BufferedReader(new InputStreamReader(webSocket.getInputStream))
    val fReq = input.readLine()

    if (fReq == null) {
      println("Пустой запрос...")
      webSocket.close()
      webServSocket.close()
      return "empty"
    }
    println("Запрос - " + fReq)
    var parameter= fReq.split(' ')(1).substring(1)

    var remonteSA=webSocket.getRemoteSocketAddress.toString.split(":")(0)
    if (remonteSA=="0.0.0.0") remonteSA=="127.0.0.1"
    println(remonteSA)

    if(parameter.length > 1 && checkInt(parameter)){
      nrOfElements = parameter.toInt

      slavePort=slavePort+1
      if(slavePort==maxSlavePort){slavePort=minSlavePort}
      try {
        val output = new PrintWriter(new OutputStreamWriter(webSocket.getOutputStream))
        val lastRow = "<tr>" + "<td colspan=\"3\"> "  + nrOfElements.toString + " точек, ожидайте результата "+ "</td></tr></table></body>"
        output.print(webPage(0)+ (webPage(1).getBytes.length + webPage(2).getBytes.length + lastRow.getBytes.length).toString + "\n\n" +
          webPage(1) + remonteSA +":"+ webPort + webPage(2) + lastRow)
        output.flush
      }
      webSocket.close()
      webServSocket.close()
      new Thread(new Runnable() {
        override def run(): Unit = {
          runSlaveApp(nrOfElements,slavePort)
        }
      }).start()
    } else {
      if(parameter=="update")
      {
        try {
          val output = new PrintWriter(new OutputStreamWriter(webSocket.getOutputStream))
          val lastRow = "</table></body>"
          output.print(webPage(0) + (webPage(1).getBytes.length + webPage(2).getBytes.length + lastRow.getBytes.length).toString + "\n\n" +
            webPage(1) + remonteSA +":"+ webPort + webPage(2) + lastRow)
          output.flush
        }
        webSocket.close()
        webServSocket.close()
      } else {
        if(parameter != "favicon.ico") {
          try {
            val output = new PrintWriter(new OutputStreamWriter(webSocket.getOutputStream))
            val lastRow = "<head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><h1>Вводите число точек в адресную строку: http://127.0.0.1:9080/*число_точек*</h1>\""
            output.print(webPage(0) + (lastRow.getBytes.length).toString + "\n\n" + lastRow)
            output.flush
          }
        }
        webSocket.close()
        webServSocket.close()
      }
    }
    return ""
  }

  def checkInt(inp:String):Boolean={try {inp.toInt; return true} catch {case _=>return false}}

  def runSlaveApp(nrOfElements:Int,slavePort:Int):Array[String]={
    val fullPath = "java -cp \"" + Properties.javaClassPath + "\" SlaveApplication " + slavePort + " " + nrOfElements
    fullPath.run()
    val slaveServSocket = new ServerSocket(slavePort)
    //ожидание ответа SlaveApp
    val slave = slaveServSocket.accept()
    val slaveReader = new BufferedReader(new InputStreamReader(slave.getInputStream))
    val comeLine = slaveReader.readLine().split('#')
    println("время расчета / значение pi - " + comeLine(0) + " / " + comeLine(1))
    webPage(2) = webPage(2) + "<tr><td>" + nrOfElements.toString + "</td><td>" + comeLine(0) + "</td><td>" + comeLine(1) + "</td></tr>"
    slaveReader.close()
    slave.close()
    slaveServSocket.close()
    return comeLine
  }


}



