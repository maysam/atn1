#!/usr/bin/env python
from optparse import OptionParser

from mas_api import MAS

# Objects defined in this file:
##
## TheoryNetwork

class TheoryNetwork:
	def __init__(self, paper_zero):
		# List of tuples, (cited paper publication_id,
		#					citing paper publication_id,
		#					depth level as integer),
		# representing papers that still need to be downloaded.
		self.__download_list = []

		# The assumption made here is that the crawler is getting
		# kicked off with the publication_id of paper zero.
		self.paper_zero = paper_zero

		# Only one datasource needed at this time, see comments in api.py
		self.api = MAS()


	def get_next_paper(self):
		if len(self.__download_list) != 0:
			return self.__download_list.pop()
		else:
			return None

	def add_papers(self, depth, cited_paper, paper_list):
		for paper in paper_list:
			self.__download_list.append((cited_paper,paper,depth))



# OptParse options:
# --generate-network <paper zero publication id>
# --update-network <Theory Network name>
#
# ex: python crawler.py --generate-network 12345667890