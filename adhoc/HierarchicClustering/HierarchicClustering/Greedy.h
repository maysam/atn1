#ifndef GREEDY_H
#define GREEDY_H
#include "MersenneTwister.h"
#include "GreedyBase.h"
#include "Node.h"
#include <cmath>
#include <iterator>
#include <iostream>
#include <vector>
#include <set>
#include <map>

using namespace std;


class Greedy : public GreedyBase{
 public:
  Greedy(MTRand *RR,int nnode,Node **ah, bool initrun);
  virtual ~Greedy();
  virtual void initiate(void);
  virtual void calibrate(void);
  virtual void tune(void);
  virtual void prepare(bool sort);
  virtual void level(Node ***,bool sort);
  virtual void move(bool &moved);
  virtual void determMove(vector<int> &moveTo);
  virtual void eigenvector(void);
  virtual void eigenfactor(void);
  virtual void collapseNodes(void);

  vector<int> danglings;
  
  int Nempty;
  vector<int> mod_empty;
  
  vector<double> mod_enter;
  vector<double> mod_exit;
  vector<double> mod_size;
  vector<double> mod_outFlow;
  vector<int> mod_members;
  
 protected:

  vector<int> modSnode;
};

#endif
