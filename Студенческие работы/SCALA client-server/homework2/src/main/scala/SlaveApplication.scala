import akka.actor._
import akka.routing.RoundRobinPool
import scala.math._
import java.io._
import java.net._
import scala.util.{Failure, Random, Success, Try}

object SlaveApplication {
  def main(args: Array[String]): Unit = {
    var portToMaster: Int = 9000
    var nrOfElements: Int = 2000

    try ( portToMaster = args(0).toInt) catch {case _ => println("Неверный номер порта (по умолчанию 9000)" ); portToMaster = 9000 }
    if(args.length > 1) try(nrOfElements=args(1).toInt) catch {case _ => println("Неверное число точек (по умолчанию 2000)" ); nrOfElements = 2000}
    else {println("Неверное число точек (по умолчанию 2000)" ); nrOfElements = 2000}
      // создать Akka-систему
      val system = ActorSystem("PiSystem")
      // создать Slave-актор
      val slave = system.actorOf(
        Props(new Slave(portToMaster,nrOfElements)),
        name = "slave")
      // начать расчет:
      slave ! Calculate
  }
  sealed trait PiMessage
  case object Calculate extends PiMessage
  case class Work(nrOfElements: Int) extends PiMessage
  case class Result(value: Double) extends PiMessage
  //case class PiApproximation(pi: Double, duration: Double) extends PiMessage

  class Worker extends Actor {
    def receive : PartialFunction[Any,Unit] = {
      case Work(nrOfElements) => sender ! Result(monteCarlo(nrOfElements)/nrOfElements ) // *4)
    }

    /*def monteCarlo(trials: Int) : Double = { //медленный расчет по формуле
      Stream
        .continually(if (sqrt(Math.pow(Random.nextDouble, 2) + Math.pow(Random.nextDouble, 2)) <= 1) 1.0 else 0.0)
        .take(trials)
        .sum / trials
    }*/
    def monteCarlo(pointNumber: Int) : Double = { //быстрый расчет
      import java.util.Random
      val r = new Random
      var circle = 0

      var i = 0
      while ( {  i < pointNumber
      }) {
        if (IsCircle(1.0, r.nextDouble, r.nextDouble)) circle += 1

        {
          i += 1; i - 1
        }
      }
      return ((4 * circle) / pointNumber.asInstanceOf[Double])
    }

    def IsCircle( radius: Double, x: Double, y: Double):Boolean = {
      return ((x * x + y * y) <= radius * radius);
    }
  }

  class Slave(portToMaster: Int,nrOfElements: Int) extends Actor {
    var pi: Double = _
    var nrOfResults: Int = _
    val start: Long = System.currentTimeMillis
    val workerRouter = context.actorOf( Props[Worker].withRouter(RoundRobinPool(nrOfElements)),
      name = "workerRouter"  )

    def receive : PartialFunction[Any,Unit] = {
      case Calculate =>
        for (i <- 0 until nrOfElements)
          workerRouter ! Work(nrOfElements)
      case Result(value) =>
        pi += value
        nrOfResults += 1
        if (nrOfResults == nrOfElements) {
          println(pi.toString)
          val socket = new Socket("localhost", portToMaster)
          val output = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream))

          output.write((System.currentTimeMillis-start).toString+"#"+pi.toString)
          output.flush()
          socket.close()
          context.stop(self)
        }
    }
  }
}


