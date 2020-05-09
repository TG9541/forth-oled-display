\ Real time clock modules DS1307 and DS3231, I2C communication
\ DS3231 is much more accurate
\ Both have 4kb eeprom, 128 pages of 32 bytes
\ Registers 0:6  :  BCD data for sec, min, hour, #day(1:7), date, month, year(0:99)

$68 CONSTANT rtc     \ slave address clock
$57 CONSTANT eeprom  \ eeprom slave address

NVM

VARIABLE bf $19 allot

: RDC   \ Read clock
   bf 7 0 rtc i2rf
;

\ Set clock reg's 6:0, BCD input
: SETC  ( YY MM DD d hh mm ss)
   bf 7 0 DO DUP ROT SWAP C! 1+ LOOP DROP
   bf 7 0 rtc i2wf ( buf-adr n reg-adr i2c-adr --)
;

\ set single clock register
: SCRG  ( b reg-adr --)
   rtc i2wb ( b reg-adr i2c-adr)
;

\ Hex output
: h. ( b --)
   BASE @ >r HEX . r> BASE ! 
;

: time
   RDC cr
   bf 2+ C@ $3F AND h. ."  : " bf 1+ C@ h. ."  : " bf C@ h.
;

: date
   RDC cr
   bf 4 + C@ h. ."  / " bf 5 + C@ h. ."  / '" bf 6 + C@ h.
;

\ EEPROM words, slave address $57

\ write n bytes from buffer to eeprom
: eew ( buf-adr n #byte #page --)
   i2s eeprom 0 i2a i2sb i2sb i2sf i2p
;

\ load buffer with n bytes from eeprom
: eer ( buf-adr n #byte #page --)
   i2s eeprom 0 i2a i2sb i2sb i2p i2s eeprom 1 i2a i2cf
;

RAM

\\ example

hex
18 3 15 4 23 55 45 setc  \ march 15 2018 23:55:45
date time
5 11 scrg				 \ set clock reg. 5 (month) to 11

Clock registers 0:6
0 sec
1 min
2 hr
3 #day (1-7)
4 day
5 month
6 year (0-99)
 
 Together with ssd1306.fs:
	: txt $" This text to display" ;	\ Compile text
	' txt 3 + DUP C@ 0 8 eew			\ write compiled text in page 8 of eeprom
	bf 20 0 8 eer						\ read 20 bytes of page 8, start at 1ste position, from eeprom
	bf dtxt								\ display text on ssd1306


