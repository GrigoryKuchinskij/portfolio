name := "homework2"
version := "0.1"
scalaVersion := "2.12.6"

Compile/mainClass := Some("Application")

libraryDependencies += "com.typesafe.akka" %% "akka-actor" % "2.5.17"
libraryDependencies += "com.typesafe.akka" %% "akka-remote" % "2.5.17"