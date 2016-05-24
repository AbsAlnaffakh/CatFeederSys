# Project brief

An Embedded system which fulfills the functionality of a smart cat feeder

The functionalities of the system where based on the beneficial features that other competitiors have missed, these are as follows:
  1. Audio playback and capture: To play a recording of the owners voice calling out the cats name, used to attract the pet towards the
     dispenced food.  
  2. A method to verify which pet is consumeing dispenced food, via the use of camera driven by the Motion bibrary 
  3. A confirmation method to inform the owner whether the pet has consumed the food or not, via the automatic generation of emails,      proofes to be useful to indicate that there is an issue preventing the pet from feeding. 
  4. Up to date interction method to set the systems settings, achieved by deploying a web page on a web server that a user can           interact with from any device connected to the same LAN, settings include the meal release times and voice recordings. These         settings are stored into files, the main programme can read those files and behave accordingly.
  
Although that the system may seem simple at first glance however it consists of several entities that work together to achieve the main aim
of the system:
  1. communication between several application has to be established i.e the system makes use of a media player.
  2. Motors where used to control meal dispencing, however the library used to allow this can only be used in C, which means
     that more than one programming technology is used.

Takes advantage of OS features:
  1. Initalises several process
  2. Takes advantage of the resource managemnt feature, by using the kernel as medium of communication between applications and           hardware through the utilisation of device drivers, this would have been much more difficult if an OS was not used, communication    between software and  hardware would have to be implemented at lower levels.
  3. Multi tasking, several application where concurrently running (Web Server, media player, motor controllers, motion libraries) this    would not have been possible if the system is to be implmented on a bare-metal.

The main programme was written in C#, however other entities have made use of other programming technologies, the web page made use of 
JavaScript, HTML and CSS, while the motor control was carried out via C.

To view the source code navigate to /CatFeederSys/CFmain.cs , the approach used was a structured programming approach however it would have been better to follow a modular OOP approach.
