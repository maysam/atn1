% AEF code by Jevin West, modified by Trevor Chambers
% January 3rd, 2013
% ATN

clear;
%% Args and params
verbose = 0; % Run verbose for intermediate out
timing = 1; % verbose timing

fin = 'TAM.net';
fout = 'TAM-AEF_IF.txt';
alpha=0.85;
epsilon=0.000000000000001; 

% Start full timer
if (timing == 1) startFull = tic; end

%% Read in (pajek format)
[N,L] = pajek_read(fin);

% Edges
mLinks = [L{1},L{2}];

% Edge weights (not currently necessary, however)
F = L{3};
clear L

% Store |V| and |E|
pn = numel(N{1});          % pn = |vertices|
ln = numel(mLinks(:,1));    % ln = |edges|

%% -- RANKING -- 
% EF timer
if (timing == 1) startRank = tic; end

% Compute impact factor
[IF] = impact_factor(mLinks, ln);

% Handle dangling nodes 
[vT,D] = dangling_nodes(mLinks,F,pn,ln);

% Compute EF (aproximation for non-ergodic networks)
[EF] = EF_undirdir(mLinks, F, pn, verbose);

if (timing == 1) elapsedRank = toc(startRank); end

%% Write out and disp
write_rankings(fout, N{2}, EF, IF, pn);

if (timing == 1)
    elapsedFull = toc(startFull);
    fprintf(' --- Timing ---\n');
    fprintf('Full run: %d sec(s)\n', elapsedFull);
    fprintf('Ranking computation: %d sec(s)\n', elapsedRank);
end
