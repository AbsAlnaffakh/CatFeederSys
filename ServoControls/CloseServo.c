// Uses POSIX functions to send and receive data from a Maestro.
// NOTE: The Maestro's serial mode must be set to "USB Dual Port".
// NOTE: You must change the 'const char * device' line below.
 
#include <fcntl.h>
#include <stdio.h>
#include <unistd.h>
 
#ifdef _WIN32
#define O_NOCTTY 0
#else
#include <termios.h>
#endif
 
// Gets the position of a Maestro channel.
// See the "Serial Servo Commands" section of the user's guide.
int maestroGetPosition(int fd, unsigned char channel)
{
  unsigned char command[] = {0x90, channel};
  if(write(fd, command, sizeof(command)) == -1)
  {
    perror("error writing");
    return -1;
  }
 
  unsigned char response[2];
  if(read(fd,response,2) != 2)
  {
    perror("error reading");
    return -1;
  }
 
  return response[0] + 256*response[1];
}
 
// Sets the target of a Maestro channel.
// See the "Serial Servo Commands" section of the user's guide.
// The units of 'target' are quarter-microseconds.
int maestroSetTarget(int fd, unsigned char channel, unsigned short target)
{
  unsigned char command[] = {0x84, channel, target & 0x7F, target >> 7 & 0x7F};
  if (write(fd, command, sizeof(command)) == -1)
  {
    perror("error writing");
    return -1;
  }
  return 0;
}
 
int main()
{

  const char * device = "/dev/ttyACM0";
  int fd = open(device, O_RDWR | O_NOCTTY);
  if (fd == -1)
  {
    perror(device);
    return 1;
  }
 
// configuring the serial port
  struct termios options;
  tcgetattr(fd, &options);
  options.c_iflag &= ~(INLCR | IGNCR | ICRNL | IXON | IXOFF);
  options.c_oflag &= ~(ONLCR | OCRNL);
  options.c_lflag &= ~(ECHO | ECHONL | ICANON | ISIG | IEXTEN);
  tcsetattr(fd, TCSANOW, &options);

 
  int position = maestroGetPosition(fd, 0);
  printf("Current position is %d.\n", position);

/*
  // sets the movement position for the servo motor by getting the current position and setting the target position
  // ? conditional statemnt if true then x: otherwise y
  // int target = (position < 1330) ? 1330 : 7000;
  //target = (position = 1330) ? 2000 : 1330; 
  //int target = (position < 6000) ? 7000 : 5000;
  printf("Setting target to %d (%d us).\n", target, target/4);
  maestroSetTarget(fd, 0, target);
  //maestroSetTarget(fd, 0, 900);*/


//this is fine for open
// maestroSetTarget(fd, 0, 5000);

//maestroSetTarget(fd, 0, 6000);

maestroSetTarget(fd, 0, 2000);

/*if (position == 5000){
maestroSetTarget(fd, 0, 2000);
}*/

// the below lines are working
 /* int target = (position < 6000) ? 7000 : 5000;
  printf("Setting target to %d (%d us).\n", target, target/4);
  maestroSetTarget(fd, 0, target);*/

  close(fd);
  return 0;
}
