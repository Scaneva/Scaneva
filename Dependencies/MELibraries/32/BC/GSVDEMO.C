/************************************************/
/*                                              */
/*  Demonstrationsprogramm für die              */
/*  Funktionen in MEGSV.DLL                     */
/*                                              */
/*  Copyright (C) Dr. Holger Kabelitz 1999-2009 */
/*  Alle Rechte vorbehalten.                    */
/*                                              */
/*  Dr. Holger Kabelitz                         */
/*  D-13507 Berlin                              */
/*                                              */
/************************************************/

#include <stdio.h>
#include <conio.h>
#include <windows.h>
#include "megsv.h"

#define ComNr 1         /* Nummer der seriellen Schnittstelle */
#define BuffSize 1000   /* Größe des Puffers, in dem die AD-Werte
                           abgelegt werden, bis sie mit dem Befehl
                           Read gelesen werden */

static char *befehle[] =
{
   "o    : SetOffset(ComNr)",
   "c    : SetCal(ComNr)",
   "z    : SetZero(ComNr)",
   "s    : SetScale(ComNr)",
   "b    : SetBipolar(ComNr)",
   "u    : SetUnipolar(ComNr)",
   "f    : SetFreq(ComNr, Freq)",
   "g    : SetGain(ComNr, Gain)",
   (char *)NULL
};

static void Ausgabe(char *str)
{
   char temp[80];

   CharToOem(str, temp);
   cputs(temp);
}

static void BefehlsUebersicht(int einzel, int taste)
{
   int i;

   if (!einzel)
      Ausgabe("Befehlsauswahl\r\n");

   for (i = 0; befehle[i] != NULL; i++)
      if (!einzel || taste == befehle[i][0])
         cprintf(einzel ? "\r\n\n%s\r\n" : "%s\r\n", befehle[i]);

   if (!einzel)
      Ausgabe("ESC  : Beenden\r\n\n");
}

static int Taste(void)
{
   if (!kbhit())
      return 0;

   return getch();
}

static void LiesZeile(char *z, int l)
{
   int k, i = 0;

   do
   {
      k = getch();

      if (k >= ' ' && k < 127)
      {
         if (i < l - 1)
         {
            z[i++] = (char)k;
            putch(k);
         }
      }
      else if (k == '\b' || k == 127)
      {
         if (i > 0)
         {
            i--;
            cputs("\b \b");
         }
      }
   } while (k != '\r');      

   z[i] = (char)0;
}


static void WerteLesen(void)
{
   double ad;

   if (GSVread(ComNr, &ad) == GSV_TRUE)
      cprintf("  %12.6f\r", ad);
}

static void FehlerMeldung(int code, int ref, int warn)
{
   if (code == ref)
      Ausgabe("-> Erfolgreich durchgeführt\r\n\n");
   else if (code == GSV_OK)
      Ausgabe("-> Unerwartete Zeitüberschreitung!\r\n\n");
   else
      Ausgabe("-> Fehler! Nicht durchgeführt\r\n\n");

   BefehlsUebersicht(0, 0);

   if (warn)
   {
      Ausgabe("NACH DIESEM BEFEHL MÜSSEN SetCal UND");
      Ausgabe(" SetZero AUSGEFÜHRT WERDEN!!!\r\n\n");
   }
}

static void TastaturAbfragen(int ca)
{
   char zeile[64];
   double freq;
   int gain;

   BefehlsUebersicht(1, ca);

   switch (ca)
   {
      case 'o':
         FehlerMeldung(GSVsetOffset(ComNr), GSV_TRUE, 0);
         break;
         
      case 'c':
         FehlerMeldung(GSVsetCal(ComNr), GSV_TRUE, 0);
         break;
         
      case 'z':
         FehlerMeldung(GSVsetZero(ComNr), GSV_TRUE, 0);
         break;
         
      case 's':
         FehlerMeldung(GSVsetScale(ComNr), GSV_TRUE, 0);
         break;
         
      case 'b':
         /* Nach GSVsetBipolar müssen GSVsetCal und */
         /* GSVsetZero ausgeführt werden!        */
         FehlerMeldung(GSVsetBipolar(ComNr), GSV_OK, 1);
         break;
         
      case 'u':
         /* Nach GSVsetUnipolar müssen GSVsetCal und */
         /* GSVsetZero ausgeführt werden!        */
         FehlerMeldung(GSVsetUnipolar(ComNr), GSV_OK, 1);
         break;
         
      case 'f':
         Ausgabe("Bitte Frequenz eingeben: ");
         LiesZeile(zeile, (int)sizeof(zeile));
         cputs("\r\n");
         if (sscanf(zeile, " %lg", &freq) != 1)
            Ausgabe("Ungültige Eingabe\r\n\n");
         else
         {
            /* Nach GSVsetFreq müssen GSVsetCal und */
            /* GSVsetZero ausgeführt werden!        */
            FehlerMeldung(GSVsetFreq(ComNr, freq), GSV_OK, 1);
         }
         while (Taste());
         break;
   
      case 'g':
         Ausgabe("Bitte Verstärkung (Gain) eingeben: ");
         LiesZeile(zeile, (int)sizeof(zeile));
         cputs("\r\n");
         if (sscanf(zeile, " %i", &gain) != 1)
            Ausgabe("Ungültige Eingabe\r\n\n");
         else
         {
            /* Nach GSVsetGain müssen GSVsetCal und */
            /* GSVsetZero ausgeführt werden!        */
            FehlerMeldung(GSVsetGain(ComNr, gain), GSV_OK, 1);
         }
         while (Taste());
         break;

      default: ;
   }
}

int main()              /* Hauptprogramm */
{
   int ch;
   long tmp;

   Ausgabe("\r\nDemonstrationsprogramm für MEGSV.DLL");
   Ausgabe("\r\nVersion der Bibliothek: ");
   tmp = GSVversion();
   cprintf("%ld.", tmp >> 16);
   tmp &= 0xFFFF;
   cprintf("%ld%ld\r\n", tmp / 10, tmp % 10);

   if (GSVactivate(ComNr, BuffSize) != GSV_OK)
   {
      Ausgabe("\r\nInitialisierung der Baugruppe fehlgeschlagen\r\n");
   }
   else
   {
      tmp = GSVmodel(ComNr);
      Ausgabe("Baugruppen-Modell: ");
      cprintf("%ld\r\n\n", tmp);

      BefehlsUebersicht(0, 0);

      do
      {
         ch = Taste();
         WerteLesen();
         TastaturAbfragen(ch);     
      } while (ch != 27);

      GSVrelease(ComNr);

      cputs("\r\n");
   }

   return 0;
}
