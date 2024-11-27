using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class G04_BlockManager : MonoBehaviour
{
    public static Action<G04_BlockManager> OnBlockSpawned;
    public static Action<G04_BlockManager> OnBlockCombined;
    public static Action<G04_BlockManager> OnStartBlockPlaced;
    public static Action<G04_BlockManager> OnSelectionChanged;

    public List<G04_CombinedBlock> GetSelectedCombinedBlocks { get { return _selectedCombinedBlocks; } }

    [SerializeField] private GameObject[] _combinedBlockPrefabs;
    [SerializeField] private GameObject[] _startBlockPrefabs;
    [SerializeField] private Vector3 _startBlockPos;

    private G04_CombinedBlock _currentStartBlock;
    private int _maxSelectedCombinedBlocks = 2;
    private List<G04_CombinedBlock> _selectedCombinedBlocks = new();
    private G04_Grid _grid;
    private G04_CombinedBlock _pickedBlock;

    void Start() {
        _grid = G04_GameManager.Instance.GetGrid;
        OnSelectionChanged?.Invoke(this);
        SpawnBlock();
    }

    private void OnEnable() {
        G04_UI.OnTurnEnded += G04_UI_OnTurnEnded;
        G04_CombinedBlock.OnGridPlacement += G04_CombinedBlock_OnGridPlacement;
    }

    private void OnDisable() {
        G04_UI.OnTurnEnded -= G04_UI_OnTurnEnded;
        G04_CombinedBlock.OnGridPlacement -= G04_CombinedBlock_OnGridPlacement;
    }

    private void G04_CombinedBlock_OnGridPlacement(G04_CombinedBlock block) {
        if (block == _currentStartBlock) {
            OnStartBlockPlaced?.Invoke(this);
            _currentStartBlock = null;
            SpawnBlock();
        }

        //ResolveImmediateEffects();
    }

    private void G04_UI_OnTurnEnded() {
        ResetSelection();
        SpawnBlock();
        ResolveEndOfTurnEffects();
        ResolveLevelUps();
        //ResolveImmediateEffects();
    }

    private void ResetSelection() {
        foreach (var combinedBlock in _selectedCombinedBlocks) {
            combinedBlock.OnSelect(false);
        }
        _selectedCombinedBlocks.Clear();
        OnSelectionChanged?.Invoke(this);
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            PickUpBlock();
        }

        if (Input.GetMouseButtonUp(0)) {
            PutDownBlock();
        }

        if (Input.GetMouseButtonDown(1)) {
            if (_pickedBlock == null) {
                SelectBlock();
            } else {
                _pickedBlock.transform.Rotate(0f, 0f, 90f);
            }
        }

        if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.Space)) {
            CombineBlocks();
        }        
    }

    private void SpawnBlock() {
        if ((_currentStartBlock != null && !_currentStartBlock.GetIsOnGrid) || G04_GameManager.Instance.GetRemainingBlocks <= 0 ) {
            return;
        }

        int rndIndex = UnityEngine.Random.Range(0, _startBlockPrefabs.Length);
        _currentStartBlock = Instantiate(_startBlockPrefabs[rndIndex], _startBlockPos, Quaternion.identity).GetComponent<G04_CombinedBlock>();
        OnBlockSpawned?.Invoke(this);
    }

    private void PickUpBlock() {
        RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero);

        if (hit.collider != null) {
            //Debug.Log("Hit: " + hit.collider.name);
            _pickedBlock = hit.collider.GetComponentInParent<G04_CombinedBlock>();
            if (_pickedBlock != null) {
                ResetSelection();
                _pickedBlock.OnPickUp();
                
                //Debug.Log("Block picked up.");
            }

        } else {
            //Debug.Log("Nothing hit!");
        }
    }
    
    private void PutDownBlock() {
        if (_pickedBlock != null) {
            _pickedBlock.OnDropDown();
            _pickedBlock = null;
        }
    }

    private void SelectBlock() {
        RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero);

        if (hit.collider != null) {
            G04_CombinedBlock combinedBlock = hit.collider.GetComponentInParent<G04_CombinedBlock>();

            if (combinedBlock != null && combinedBlock.GetIsOnGrid) {
                if (_selectedCombinedBlocks.Contains(combinedBlock)) { // deselect if selected
                    combinedBlock.OnSelect(false);
                    _selectedCombinedBlocks.Remove(combinedBlock);
                    OnSelectionChanged?.Invoke(this);

                }
                else if (_selectedCombinedBlocks.Count < _maxSelectedCombinedBlocks) { // select if not selected & not max selected
                    combinedBlock.OnSelect(true);
                    _selectedCombinedBlocks.Add(combinedBlock);
                    OnSelectionChanged?.Invoke(this);
                }

                //Debug.Log("Selected blocks count: " + _selectedCombinedBlocks.Count);
            }
        }
    }

    public void CombineBlocks() {
        if (G04_GameManager.Instance.GetRemainingCombines <= 0) {
            GameLog.Instance.UpdateLog("No combines remaining. End turn to refresh.");
            return;
        }

        if(_selectedCombinedBlocks.Count == _maxSelectedCombinedBlocks) {
            // atm works only for combining 2
            var combinedBlock1 = _selectedCombinedBlocks[0];
            var combinedBlock2 = _selectedCombinedBlocks[1];
            ResetSelection();

            if(_grid.CheckCombinedBlocksAdjacent(combinedBlock1, combinedBlock2)) {
                if (combinedBlock1.GetBlockTier != combinedBlock2.GetBlockTier) {
                    GameLog.Instance.UpdateLog("Can't combine. Block tiers don't match.");
                    return;
                }

                G04_CombinedBlock.BlockType newType = G04_CombinedBlock.BlockType.None;

                //TODO: change to something better when time
                if (combinedBlock1.GetBlockType == combinedBlock2.GetBlockType) {
                    newType = combinedBlock1.GetBlockType;
                } else if (combinedBlock1.GetBlockTier == G04_CombinedBlock.BlockTier.One) {
                    if (combinedBlock1.GetBlockType == G04_CombinedBlock.BlockType.Blue ||
                                    combinedBlock2.GetBlockType == G04_CombinedBlock.BlockType.Blue) {
                        if (combinedBlock1.GetBlockType == G04_CombinedBlock.BlockType.Red ||
                                    combinedBlock2.GetBlockType == G04_CombinedBlock.BlockType.Red) {
                            newType = G04_CombinedBlock.BlockType.Purple;
                        } else if (combinedBlock1.GetBlockType == G04_CombinedBlock.BlockType.Yellow ||
                                    combinedBlock2.GetBlockType == G04_CombinedBlock.BlockType.Yellow) {
                            newType = G04_CombinedBlock.BlockType.Green;
                        }
                    } else {
                        newType = G04_CombinedBlock.BlockType.Orange;
                    }
                } else if (combinedBlock1.GetBlockTier == G04_CombinedBlock.BlockTier.Two) {
                    newType = G04_CombinedBlock.BlockType.White;
                }

                var blocks = new List<G04_Block>();
                blocks.AddRange(combinedBlock1.GetBlocks);
                blocks.AddRange(combinedBlock2.GetBlocks);


                Vector3[] blocksPos = blocks.Select(x => x.transform.position).ToArray();
                Vector3 newPos = GetCombinedBlockPosition(blocksPos);
                G04_CombinedBlock newCombinedBlock = newType switch {
                    G04_CombinedBlock.BlockType.Blue => Instantiate(_combinedBlockPrefabs[1], newPos, Quaternion.identity).GetComponent<G04_CombinedBlock>(),
                    G04_CombinedBlock.BlockType.Red => Instantiate(_combinedBlockPrefabs[2], newPos, Quaternion.identity).GetComponent<G04_CombinedBlock>(),
                    G04_CombinedBlock.BlockType.Yellow => Instantiate(_combinedBlockPrefabs[3], newPos, Quaternion.identity).GetComponent<G04_CombinedBlock>(),
                    G04_CombinedBlock.BlockType.Green => Instantiate(_combinedBlockPrefabs[4], newPos, Quaternion.identity).GetComponent<G04_CombinedBlock>(),
                    G04_CombinedBlock.BlockType.Orange => Instantiate(_combinedBlockPrefabs[5], newPos, Quaternion.identity).GetComponent<G04_CombinedBlock>(),
                    G04_CombinedBlock.BlockType.Purple => Instantiate(_combinedBlockPrefabs[6], newPos, Quaternion.identity).GetComponent<G04_CombinedBlock>(),
                    G04_CombinedBlock.BlockType.White => Instantiate(_combinedBlockPrefabs[7], newPos, Quaternion.identity).GetComponent<G04_CombinedBlock>(),
                    _ => Instantiate(_combinedBlockPrefabs[0], newPos, Quaternion.identity).GetComponent<G04_CombinedBlock>(),
                };


                for (int i = 0; i < blocks.Count; i++) {
                    newCombinedBlock.AddBlock(blocks[i]);
                    _grid.UpdateCellBlockInfo(blocks[i], _grid.GetBlockCoor(blocks[i]));
                }

                var newLevel = Mathf.Max(combinedBlock1.GetBlockLevel, combinedBlock2.GetBlockLevel);
                var newBonusValue = combinedBlock1.GetBlockBonusValue + combinedBlock2.GetBlockBonusValue;
                var newMultiplier = combinedBlock1.GetBlockMultiplier + combinedBlock2.GetBlockMultiplier - 1f; // -1 because multiplier starts at 1 on a new block
                newCombinedBlock.SetLevel(newLevel);
                newCombinedBlock.SetMultiplier(newMultiplier);
                newCombinedBlock.SetBonusValue(newBonusValue);
                newCombinedBlock.UpdateValues();

                Destroy(combinedBlock1.gameObject);
                Destroy(combinedBlock2.gameObject);

                OnBlockCombined?.Invoke(this);

            } else {
                GameLog.Instance.UpdateLog("Selected blocks are not adjacent.");
            }
        } else {
            GameLog.Instance.UpdateLog(_maxSelectedCombinedBlocks + " blocks need to be selected.");
        }
    }

    private Vector3 GetMouseWorldPosition() {
        var mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    public static Vector3 GetCombinedBlockPosition(Vector3[] blocksPos) {
        if (blocksPos.Length == 0) return Vector3.zero;

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var blockPos in blocksPos) {

            minX = Mathf.Min(minX, blockPos.x);
            maxX = Mathf.Max(maxX, blockPos.x);
            minY = Mathf.Min(minY, blockPos.y);
            maxY = Mathf.Max(maxY, blockPos.y);
        }

        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;

        //Debug.Log("X: " + centerX + "Y " + centerY);

        return new Vector3(centerX, centerY, 0f);
    }

    private void ResolveEndOfTurnEffects() {
        var blockStartLvlDict = new Dictionary<G04_CombinedBlock, int>();
        G04_CombinedBlock[] allBlocks = _grid.GetAllCombinedBlocksOnGrid();
        foreach (var combinedBlock in allBlocks) {
            blockStartLvlDict[combinedBlock] = combinedBlock.GetBlockLevel;
        }

        foreach (var combinedBlock in allBlocks) {
            // resolve target: self effects from this block
            var selfEffects = combinedBlock.GetBlockEffects().Where(x => x.GetResolveType == G04_BlockEffect.ResolveType.Turn
                                                                    && x.GetTargetType == G04_BlockEffect.TargetType.Self);
            foreach (var effect in selfEffects) {
                effect.ResolveEffect(combinedBlock, blockStartLvlDict[combinedBlock]);
            }

            // resolve target: other effects from adjacent blocks
            G04_CombinedBlock[] adjComboBlocks = _grid.GetAdjacentCombinedBlocks(combinedBlock);        
            foreach (var adjComboBlock in adjComboBlocks) {
                var otherEffects = adjComboBlock.GetBlockEffects().Where(x => x.GetResolveType == G04_BlockEffect.ResolveType.Turn
                                                                        && x.GetTargetType == G04_BlockEffect.TargetType.Other);
                foreach (var effect in otherEffects) {
                    effect.ResolveEffect(combinedBlock, blockStartLvlDict[adjComboBlock]);
                }
            }
            combinedBlock.UpdateValues();
        }
    }

    private void ResolveImmediateEffects() {
        var blockStartLvlDict = new Dictionary<G04_CombinedBlock, int>();
        G04_CombinedBlock[] allBlocks = _grid.GetAllCombinedBlocksOnGrid();
        foreach (var combinedBlock in allBlocks) {
            blockStartLvlDict[combinedBlock] = combinedBlock.GetBlockLevel;
        }

        foreach (var combinedBlock in allBlocks) {
            // resolve target: self effects from this block
            var selfEffects = combinedBlock.GetBlockEffects().Where(x => x.GetResolveType == G04_BlockEffect.ResolveType.Immediate
                                                                    && x.GetTargetType == G04_BlockEffect.TargetType.Self);
            foreach (var effect in selfEffects) {
                effect.ResolveEffect(combinedBlock, blockStartLvlDict[combinedBlock]);
            }

            // resolve target: other effects from adjacent blocks
            G04_CombinedBlock[] adjComboBlocks = _grid.GetAdjacentCombinedBlocks(combinedBlock);        
            foreach (var adjComboBlock in adjComboBlocks) {
                var otherEffects = adjComboBlock.GetBlockEffects().Where(x => x.GetResolveType == G04_BlockEffect.ResolveType.Immediate
                                                                        && x.GetTargetType == G04_BlockEffect.TargetType.Other);
                foreach (var effect in otherEffects) {
                    effect.ResolveEffect(combinedBlock, blockStartLvlDict[adjComboBlock]);
                }
            }            
            combinedBlock.UpdateValues();
        }
    }
    private void ResolveLevelUps() {
        G04_CombinedBlock[] allBlocks = _grid.GetAllCombinedBlocksOnGrid();
        foreach (var combinedBlock in allBlocks) {
            combinedBlock.SetLevel(combinedBlock.GetBlockLevel + 1);
        }        
    }
}
