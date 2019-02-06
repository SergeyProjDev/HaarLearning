import serial
import serial.tools.list_ports as port_list
import cv2
import os
import urllib.request
import numpy as np
import csv
import sys
import matplotlib.pyplot as plt
from tkinter import *
from tkinter.filedialog import askopenfilename
from PIL import Image, ImageOps


useArduino = False;

#get serialization and parce it (url)
def getSerializedURL():
	with open('data.csv', newline='') as f:
		reader = csv.reader(f)
		for row in reader:
			url = str(row)
	url = url.replace("[", "")
	url = url.replace("]", "")
	url = url.replace("'", "")
	return url


#select url (getting key)
def askURL(url):
	while True:
		print ("  Press:\n 1. http://", url, 
					   "\n 2. Enter new url")

		#check choice 1 or 2
		key = input()

		#selected serializated
		if (key == "1"): 
			url1 = "http://"+url+"/shot.jpg"
			try:#check url exists
				thepage = urllib.request.urlopen(url1)
				return "http://"+url+"/shot.jpg"
			except:
				print ("URL not found...\n")
				continue

		#selected new
		if (key == "2"):
			url1 = input("Enter url: http://")
			url2 = "http://"+url1+"/shot.jpg"
			try:
				thepage = urllib.request.urlopen(url2)
			except:
				print ("URL doesn`t exist.\n")
				continue

			with open('data.csv', 'w', newline='') as a:
				writer = csv.writer(a)
				writer.writerow([url1])
			return "http://"+url1+"/shot.jpg"

		print ("Wrong value\n")


#GET COM port
def getCOMPort():
	ports = list(port_list.comports())
	for p in ports:
		print ("   ", p)
	return int(input ("Enter ONLY port NUMBER:   COM"))


#init Port
def initPort(Port):
	arduinoSerialData = serial.Serial()
	arduinoSerialData.port = "COM"+str(Port)
	arduinoSerialData.baudrate = 9600
	arduinoSerialData.timeout = 1
	arduinoSerialData.setDTR(False)
	arduinoSerialData.open()
	return arduinoSerialData


#ask to use arduino
def arduino():
	while True:
		print("Do you want to use Arduino? (y/n)")
		inp = input()
		if (inp == "y"):
			return True
		elif (inp == "n"):
			return False
		else:
			continue


#getting haar.xml path
def chooseFileXML():
	print ("Now choose Cascade Haar file(.xml)")
	window = Tk()
	window.withdraw()
	filePath = askopenfilename(
		title = "Choose Cascade Haar file(.xml)",
		filetypes = [('xml', '.xml')]
	)
	xmlPath = cv2.CascadeClassifier(filePath)
	window.destroy()
	print (filePath)
	return xmlPath


#each img processing
def processImgs(url, handCascade, arduinoSerialData = 0):
	while True:
		#Get img
		imgResp = urllib.request.urlopen(url)
		imgNp = np.array(bytearray(imgResp.read()), dtype=np.uint8)
		cvimg = cv2.imdecode(imgNp, -1)

		#Mirror
		imPil = Image.fromarray(cvimg)
		imPil = ImageOps.mirror(imPil)
		cvimg = np.array(imPil)

		#analys
		gray = cv2.cvtColor(cvimg, cv2.COLOR_RGB2GRAY)
		hand = handCascade.detectMultiScale(gray, 1.15, 20)
		count = 0
		for (x, y, w, h) in hand:
			count = count + 1
			cv2.rectangle(cvimg, (x,y), (x+w, y+h), (0,0,255), 5)
			roiGray = gray[y: y+h, x:x+w]
			roiColor = cvimg[y: y+h, x:x+w]
		
		if (useArduino):
			arduinoSerialData.write(str.encode(str(count)+"\n"))
	
		#show
		cv2.imshow('Show objects! (Press ESC to exit)', cvimg)
		if cv2.waitKey(10) == 27:
			sys.exit()


# Program start point! #
def main():
	#ip WebCam url
	url = getSerializedURL()
	url = askURL(url)

	useArduino = arduino()
	if (useArduino):
		port = getCOMPort()
		arduinoSerialData = initPort(port)

	handCascade = chooseFileXML()

	processImgs(url, handCascade)
	print ("Thanks for using!")


print ("Use \"IPWebcam\" on your Android. Start server and print URL:\n")
main()