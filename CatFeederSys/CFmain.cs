using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Text;

namespace CatFeederSys
{
	class CatFeederMain
	{
		static void Main()
		{
		   //this statically sets the OS time upon programme launch, this is only done for testing purposes
		   ExecuteCommandLine("date --set '10:00:00' "); // sets the current time to 10am for testing purposes

		   //start of by reading variables for meal dispence
		   DateTime TimeVal = new DateTime();//to store time values and to get the time now
		 
		   int CatCalls = 0; // counts the number o;	   
            
		   string Meal1Hours  = ""; // first meal
		   string Meal1Minute = ""; // first meal
           string Meal2Hours  = ""; // second meal
		   string Meal2Minute = ""; // second meal

		   string OwnerEmail = ""; //saves the users Email

		  // uses the ReadFile function to retrieve data from specific lines
		   Meal1Hours  = ReadFile(1);  // time for meal 1 is in 1st line
		   Meal1Minute = ReadFile(2);  // time for meal 1 is in 2nd line
		   Meal2Hours  = ReadFile(3);  // time for meal 2 is in 3rd line
		   Meal2Minute = ReadFile(4);  // time for meal 2 is in 4th line

		   while(true) // an infinite loop that continues to execute, once the time is known it is better to let the system sleep rather than keep looping for nothing
		   {
			TimeVal = DateTime.Now; //always update time
			
			// this function checls whether the time now is when a meal should be released
			if ((ReleaseMeal( Meal1Hours, Meal1Minute, TimeVal) == true) || (ReleaseMeal( Meal1Hours, Meal1Minute, TimeVal) == true ))
			{
			
			  PlayAudio(); // call the cat over for some food
			  CatCalls =+1; // cat called once
			  //Updatedisplay(); // yet to decide upon contents
			  
			  //send email, requries the mail client to be setup first
			  OpenServo(); // open the flap to release food
			  Thread.Sleep(10000); // waite for ten seconds 
			  CloseServo(); // close the flap
              Thread.Sleep(50000); // waite for ten seconds

			}

    
			if (CheckFoodAfterIntegrals(Meal1Hours, Meal1Minute, TimeVal, CatCalls == true))
			{
			   // should check for food here then play audio depnding if the food is consumed or not
			   // as in read from the weight sensor here
			   // if the food is eaten then we should email the owner and reset counter
		       // if the food is not eaten then increment counter and call to the cat again
               //to identify wehter the food is consumed or not, use the pressure sensor attached to the I/O shield

			   PlayAudio(); // call out to the cat again
			   CatCalls =+1; // to track how many times a callout was made
			}
			
            // this process will repeat five times after the intial call out
			if(CheckFoodAfterIntegrals(Meal2Hours, Meal2Minute, TimeVal, CatCalls == true))
			{
			   PlayAudio(); // call out to the cat again
			   CatCalls =+1; 
			}

			if (CatCalls == 6) // if the cat has been called out to 6 times then
			{
			   //inform the owner by email that food is not eaten	
			  CatCalls = 0; //Reset counter
			}
			
			IdentifyLatestFile(); // used to identify the latest image captured to attach to the email

		   }
		   
		  
		}

        //used as part of checking whether food is consumed or not (needs testing)
		static bool CheckFoodAfterIntegrals(string hours, string minutes, DateTime curr, int CatCalls)
		{
		  int h2i = Int32.Parse(hours);
          int m2i = Int32.Parse(minutes);
		  
		  //TRY TO ADD BY TEN RATHER THAN MULTIPLY
		  if((h2i == curr.Hour) && (CatCalls * 10 == curr.Minute))
		  {
		     return true;
		  }
		  return false;
        }

        //used to compare times in file to time now (tested)
		static bool ReleaseMeal(string hours, string minutes, DateTime curr)
		{
		    int h2i = Int32.Parse(hours);
            int m2i = Int32.Parse(minutes); 

		    if((h2i == curr.Hour) && (m2i == curr.Minute))
		      {
		         return true;
		      }

		  return false;
		}

	   

		//used to execute bash/shell commands (tested)
		static void ExecuteCommandLine(string command)
		{
		  Process proc = new System.Diagnostics.Process();
		  proc.StartInfo.FileName= "/bin/bash";
		  proc.StartInfo.Arguments= " -c \" " + command +" \"" ; 
	      proc.StartInfo.UseShellExecute =false;
		  proc.StartInfo.RedirectStandardOutput= true;
		  //processes.Add(proc);
		  //processes.Exited += (s, e) => { processes.Remove(proc); proc.Dispose(); };// waits for a process to exit then removes it
		  proc.Start();
		  proc.WaitForExit();
		  proc.Close();
		}

		//List<Process> processes = new List<Process>();// a list of processes to keep track of the ones running

		//Execute a bash/shell command and return a string
		static string ExecuteCommandLineWithReturn(string command)
		{
		  string ReturnedValue = "";
		  Process proc = new System.Diagnostics.Process();
		  proc.StartInfo.FileName= "/bin/bash";
		  proc.StartInfo.Arguments= " -c \" " + command +" \"" ; 
		  proc.StartInfo.UseShellExecute =false;
		  proc.StartInfo.RedirectStandardOutput= true;
		  proc.Start();
		  ReturnedValue = proc.StandardOutput.ReadToEnd();//
		  proc.WaitForExit();// wait unitl the process finishes doing whatever its doing
		  return ReturnedValue;
		}
        
		//used to control the servo motor
		static void OpenServo()
		{
			ExecuteCommandLine( "cd /home/user/CatFeederSysBETAv1.4/ServoControls; ./OpenServo");
		}

		//used to control the servo motor
		static void CloseServo()
		{
			ExecuteCommandLine( "cd /home/user/CatFeederSysBETAv1.4/ServoControls; ./CloseServo");
		}
		

		// reads from the stated line (tested)
		static string ReadFile(int LineNum)
		{
		  int counter = 0 ;
		  string line;
    
		  System.IO.StreamReader file = 
				new System.IO.StreamReader(@"/home/user/CatFeederSysBETAv1.4/Settings.txt");  // change this
		
		  while((line=file.ReadLine()) !=null)
		  {
		    counter++;
		    if (counter == LineNum)
                       {   break; }
		  }		
		
		file.Close();
		return line;          		

		}

        // the motion library is always runing and capturing images constantly, we are onlyu intereseted in the most recent captured image
        static string IdentifyLatestFile()
        {
            var directory = new System.IO.DirectoryInfo("/home/user/CatFeederSys/");
            var myFile = (from f in directory.GetFiles()
            orderby f.LastWriteTime descending
            select f).First();
            DateTime dtCreationTime = myFile.CreationTime;	  
            Console.WriteLine("the name of the latest file is:   " +  myFile);	
		    return myFile;
        }

        //To play a recording of the owners voice
		static void PlayAudio()
		{
		  ExecuteCommandLine("mplayer /home/user/CatFeederSysBETAv1.4/CatCall.wav");
		}

		//To send the display information
		static void Updatedisplay(string[] DisplayData)// meal number/ next meal time
		{

		}

		// yet to decide on call and arguments
	    static void SendEmail() 
		{

		}

	}
}
