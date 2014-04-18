from operator import itemgetter, attrgetter
from os.path import isfile
from os import access, R_OK
from time import strftime
import socket

global score_list, cache_path, crash_path
score_list = []
cache_path = './.wumpus-server-scores'
crash_path = './.wumpus-server-crashes'

class Score:
	__slots__ = ['name', 'time', 'turns', 'points', 'flag']

	def __init__(self):
		self.name = ''
		self.time = 0
		self.turns = 0
		self.points = 0
		self.flag = False

	def __str__(self):
		return self.name +';'+ str(self.points) +':'+ str(self.turns) +'|'+ str(self.time) +':'+ '\n'

def deserialize(input_string):
	global score_list
	value_string = ''
	temp_score = Score()
	for char in input_string:
		if char == ';':
			temp_score.name = value_string
			value_string = ''
		elif char == ':':
			temp_score.points = int(value_string)
			value_string = ''
		elif char == '|':
			temp_score.turns = int(value_string)
			value_string = ''
		elif char == '\n':
			temp_score.time = int(value_string)
			value_string = ''
			score_list.append(temp_score)
			temp_score = Score()
		else:
			value_string += char

def load_cache():
	global cache_path
	if isfile(cache_path) and access(cache_path, R_OK):
		with open(cache_path, 'r') as score_cache:
			deserialize(score_cache.read())

def write_cache():
	global cache_path, score_list
	with open(cache_path, 'w') as score_cache:
		for score in score_list:
			score_cache.write(str(score))

def trim_list():
	global score_list
	score_list = sorted(score_list, key=attrgetter('points'))
	score_list.reverse()
	for i in range(10):
		score_list[i].flag = True
	score_list = sorted(score_list, key=attrgetter('turns'))
	for i in range(10):
		score_list[i].flag = True
	score_list = sorted(score_list, key=attrgetter('time'))
	for i in range(10):
		score_list[i].flag = True
	for i in score_list:
		if not i.flag:
			score_list.remove(i)

def manage_socket():
	global score_list
	sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
	sock.bind(('127.0.0.1', 5005))
	while True:
		data, addr = sock.recvfrom(256)
		deserialize(data)
		trim_list()

def main():
	global crash_path
	try:
		load_cache()
		manage_socket()
		write_cache()
	except Exception as e:
		with open(crash_path, 'a') as crash_log:
			crash_log.write('%Y:%m:%d:%H:%M:%S ' + e + '\n')

if __name__ == '__main__':
	main()

